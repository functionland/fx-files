namespace Functionland.FxFiles.Client.Shared.Services.Implementations
{
    public class FakeFileService : IFileService
    {
        public IStringLocalizer<AppStrings> StringLocalizer { get; set; } = default!;
        private ConcurrentBag<FsArtifact> _files = new ConcurrentBag<FsArtifact>();
        public TimeSpan? ActionLatency { get; set; }
        public TimeSpan? EnumerationLatency { get; set; }

        public FakeFileService(IServiceProvider serviceProvider, IEnumerable<FsArtifact> files, TimeSpan? actionLatency = null, TimeSpan? enumerationLatency = null)
        {
            _files.Clear();
            StringLocalizer = serviceProvider.GetRequiredService<IStringLocalizer<AppStrings>>();
            ActionLatency = actionLatency ?? TimeSpan.FromSeconds(2);
            EnumerationLatency = enumerationLatency ?? TimeSpan.FromMilliseconds(10);
            foreach (var file in files)
            {
                _files.Add(file);
            }
        }

        public async Task LatencyActionAsync()
        {
            if (ActionLatency is not null)
                await Task.Delay(ActionLatency.Value);
        }

        public async Task LatencyEnumerationAsync()
        {
            if (EnumerationLatency is not null)
                await Task.Delay(EnumerationLatency.Value);
        }

        public async Task CopyArtifactsAsync(FsArtifact[] artifacts, string destination, bool overwrite = false, CancellationToken? cancellationToken = null)
        {
            foreach (var artifact in artifacts)
            {
                await LatencyEnumerationAsync();
                var newPath = Path.Combine(destination, artifact.Name);
                if (!overwrite)
                    CheckIfArtifactExist(newPath);

                var details = _files.Where(f => f.FullPath.StartsWith(artifact.FullPath));
                foreach (var detail in details)
                {
                    var detailPath = detail.FullPath.Replace(artifact.FullPath, newPath);
                    var newArtifact = new FsArtifact(detailPath, detail.Name, artifact.ArtifactType, artifact.ProviderType)
                    {
                        Size = artifact.Size,
                        LastModifiedDateTime = artifact.LastModifiedDateTime
                    };
                    //if (overwrite)
                    //    await DeleteArtifactsAsync(new[] { newArtifact }, cancellationToken);
                    if (!_files.Any(c => c.FullPath == newArtifact.FullPath))
                        _files.Add(newArtifact);
                }
            }
        }


        private void CheckIfArtifactExist(string newPath)
        {
            if (ArtifacExist(newPath))
                throw new ArtifactAlreadyExistsException(StringLocalizer[nameof(AppStrings.ArtifactAlreadyExistsException)]);
        }

        private bool ArtifacExist(string newPath)
        {
            StringComparer comparer = StringComparer.OrdinalIgnoreCase;
            return _files.Any(f => comparer.Compare(f.FullPath, newPath) == 0);
        }
        private bool CheckIfNameHasInvalidChars(string name)
        {
            var invalidChars = Path.GetInvalidFileNameChars();

            foreach (var invalid in invalidChars)
                if (name.Contains(invalid)) return true;
            return false;
        }

        public async Task<FsArtifact> CreateFileAsync(string path, Stream stream, CancellationToken? cancellationToken = null)
        {
            path = path.Replace("/", "\\");
            if (string.IsNullOrWhiteSpace(stream?.ToString()))
                throw new StreamNullException(StringLocalizer.GetString(AppStrings.StreamFileIsNull));

            if (string.IsNullOrWhiteSpace(path))
                throw new ArtifactPathNullException(StringLocalizer.GetString(AppStrings.ArtifactPathIsNull, "file"));

            var fileName = Path.GetFileNameWithoutExtension(path);

            if (string.IsNullOrWhiteSpace(fileName))
                throw new ArtifactNameNullException(StringLocalizer.GetString(AppStrings.ArtifactNameIsNull, "file"));

            if (CheckIfNameHasInvalidChars(fileName))
                throw new ArtifactInvalidNameException(StringLocalizer.GetString(AppStrings.ArtifactNameHasInvalidChars, "file"));

            await LatencyActionAsync();
            if (path is null) throw new Exception();

            CheckIfArtifactExist(path);

            FsArtifact artifact = CreateArtifact(path, stream.GetHashCode().ToString());
            _files.Add(artifact);
            return artifact;

        }

        private static FsArtifact CreateArtifact(string path, string? contentHash)
        {
            var originDevice = $"{Environment.MachineName}-{Environment.UserName}";
            return new FsArtifact(path, Path.GetFileName(path), FsArtifactType.File, FsFileProviderType.InternalMemory)
            {
                FileExtension = Path.GetExtension(path),
                OriginDevice = originDevice,
                ThumbnailPath = path,
                ContentHash = contentHash,
                LastModifiedDateTime = DateTimeOffset.Now.ToUniversalTime(),
                Size = 20
            };
        }

        public async Task<List<FsArtifact>> CreateFilesAsync(IEnumerable<(string path, Stream stream)> files, CancellationToken? cancellationToken = null)
        {
            var originDevice = $"{Environment.MachineName}-{Environment.UserName}";
            var addedFiles = new List<FsArtifact>();
            foreach (var artifact in from file in files
                                     let artifact = CreateArtifact(file.path, file.stream.GetHashCode().ToString())
                                     select artifact)
            {
                CheckIfArtifactExist(artifact.FullPath);
                addedFiles.Add(artifact);
            }
            foreach (var file in addedFiles)
            {
                await LatencyEnumerationAsync();
                _files.Add(file);
            }
            return addedFiles;
        }

        public async Task<FsArtifact> CreateFolderAsync(string path, string folderName, CancellationToken? cancellationToken = null)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArtifactPathNullException(StringLocalizer.GetString(AppStrings.ArtifactPathIsNull, "folder"));


            if (string.IsNullOrWhiteSpace(folderName))
                throw new ArtifactNameNullException(StringLocalizer.GetString(AppStrings.ArtifactNameIsNull, "folder"));

            if (CheckIfNameHasInvalidChars(folderName))
                throw new ArtifactInvalidNameException(StringLocalizer.GetString(AppStrings.ArtifactNameHasInvalidChars, "folder"));


            await LatencyActionAsync();
            if (path is null) throw new Exception();

            var finalPath = Path.Combine(path, folderName);
            CheckIfArtifactExist(finalPath);
            return await CreateFolder(finalPath, folderName, cancellationToken);
        }

        private async Task<FsArtifact> CreateFolder(string path, string folderName, CancellationToken? cancellationToken, bool beOverWritten = false)
        {

            var originDevice = $"{Environment.MachineName}-{Environment.UserName}";
            var artifact = new FsArtifact(path, folderName, FsArtifactType.Folder, FsFileProviderType.InternalMemory)
            {
                OriginDevice = originDevice,
                LastModifiedDateTime = DateTimeOffset.Now.ToUniversalTime(),
            };
            if (beOverWritten)
                await DeleteArtifactsAsync(new[] { artifact }, cancellationToken);
            _files.Add(artifact);
            return artifact;
        }

        public async Task DeleteArtifactsAsync(FsArtifact[] artifacts, CancellationToken? cancellationToken = null)
        {
            var finalBag = new ConcurrentBag<FsArtifact>();
            var excludedPaths = new List<string>();
            foreach (var artifact in artifacts)
            {
                foreach (var file in _files)
                {
                    if (file.FullPath.StartsWith(artifact.FullPath))
                    {
                        excludedPaths.Add(file.FullPath);
                    }
                }

            }
            foreach (var file in _files)
            {
                if (!excludedPaths.Contains(file.FullPath))
                    finalBag.Add(file);
            }
            _files = finalBag;
        }

        public async IAsyncEnumerable<FsArtifact> GetArtifactsAsync(string? path = null, string? searchText = null, CancellationToken? cancellationToken = null)
        {
            if (!string.IsNullOrWhiteSpace(path))
                path = path.Replace("/", "\\");
            IEnumerable<FsArtifact> files = _files;
            if (!string.IsNullOrWhiteSpace(path))
                files = files.Where(
                    f => string.Equals(Path.GetDirectoryName(f.FullPath), path, StringComparison.CurrentCultureIgnoreCase)
                    && !string.Equals(f.FullPath, path, StringComparison.CurrentCultureIgnoreCase));

            if (!string.IsNullOrWhiteSpace(searchText))
                files = files.Where(f => f.Name.Contains(searchText));


            foreach (var file in files)
            {
                await LatencyEnumerationAsync();
                yield return file;
            }

        }

        //TODO : Implement this GetArtifactAsync method to return a single artifact
        public Task<FsArtifact> GetFsArtifactAsync(string? path, CancellationToken? cancellationToken = null)
        {
            return null;
        }

        public async Task<Stream> GetFileContentAsync(string filePath, CancellationToken? cancellationToken = null)
        {
            await LatencyActionAsync();
            string streamPath;
            if (Path.GetExtension(filePath).ToLower() == ".jpg" ||
                Path.GetExtension(filePath).ToLower() == ".png" ||
                Path.GetExtension(filePath).ToLower() == ".jpeg"
                )
            {
                streamPath = "/Files/fake-pic.jpg";
            }
            else
            {
                streamPath = "/Files/test.txt";
            }


            using FileStream fs = File.Open(streamPath, FileMode.Open);
            return fs;

        }

        public async Task MoveArtifactsAsync(FsArtifact[] artifacts, string destination, bool overwrite = false, CancellationToken? cancellationToken = null)
        {
            await CopyArtifactsAsync(artifacts, destination, overwrite, cancellationToken);
            await DeleteArtifactsAsync(artifacts, cancellationToken);
        }

        public async Task RenameFileAsync(string filePath, string newName, CancellationToken? cancellationToken = null)
        {
            filePath = filePath.Replace("/", "\\");
            foreach (var articat in _files)
            {
                await LatencyEnumerationAsync();
                if (string.Equals(articat.FullPath, filePath, StringComparison.CurrentCultureIgnoreCase))
                {
                    var directoryName = Path.GetDirectoryName(articat.FullPath);
                    if (string.IsNullOrEmpty(Path.GetExtension(newName)))
                    {
                        var oldNameExtention = Path.GetExtension(articat.Name);
                        if (!string.IsNullOrEmpty(oldNameExtention))
                        {
                            newName = newName + oldNameExtention;
                        }
                    }
                    var newPath = Path.Combine(directoryName, newName);

                    if (ArtifacExist(newPath))
                        throw new ArtifactAlreadyExistsException(StringLocalizer.GetString(AppStrings.ArtifactAlreadyExistsException, "file"));

                    articat.FullPath = newPath;
                    articat.Name = newName;
                    break;
                }
            }
        }

        public async Task RenameFolderAsync(string folderPath, string newName, CancellationToken? cancellationToken = null)
        {
            folderPath = folderPath.Replace("/", "\\");
            var oldFolderName = Path.GetFileName(folderPath.TrimEnd(Path.DirectorySeparatorChar));
            foreach (var artifact in _files)
            {
                await LatencyEnumerationAsync();
                var artifactName = Path.GetFileName(artifact.FullPath.TrimEnd(Path.DirectorySeparatorChar));
                if (string.Equals(artifactName, oldFolderName, StringComparison.CurrentCultureIgnoreCase))
                {

                    artifact.FullPath = Path.Combine(artifact.FullPath.Substring(0, artifact.FullPath.LastIndexOf(Path.DirectorySeparatorChar)), newName);
                    artifact.Name = newName;
                }
                else if (artifact.FullPath.ToLower().StartsWith(folderPath.ToLower()))
                {
                    artifact.FullPath = artifact.FullPath.Replace($"{Path.DirectorySeparatorChar}{oldFolderName}{Path.DirectorySeparatorChar}", $"{Path.DirectorySeparatorChar}{newName}{Path.DirectorySeparatorChar}");
                }

            }
        }

        public async Task<List<FsArtifactChanges>> CheckPathExistsAsync(List<string?> paths, CancellationToken? cancellationToken = null)
        {
            var fsArtifactList = new List<FsArtifactChanges>();

            foreach (var currentPath in paths)
            {
                var path = currentPath.Replace("/", "\\");
                if (string.IsNullOrWhiteSpace(path))
                    throw new ArtifactPathNullException(StringLocalizer.GetString(AppStrings.ArtifactPathIsNull, ""));

                var artifactIsExist = ArtifacExist(path);

                var fsArtifact = new FsArtifactChanges()
                {
                    ArtifactFullPath = path,
                };

                if (artifactIsExist)
                {
                    fsArtifact.IsPathExist = true;
                }
                else
                {
                    fsArtifact.IsPathExist = false;
                    fsArtifact.FsArtifactChangesType = FsArtifactChangesType.Delete;
                }

                if (artifactIsExist)
                {
                    fsArtifact.LastModifiedDateTime = DateTimeOffset.Now;
                }

                fsArtifactList.Add(fsArtifact);
            }

            return fsArtifactList;
        }
    }
}
