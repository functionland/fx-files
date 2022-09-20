using Functionland.FxFiles.Shared.Models;
using System.Collections.Concurrent;
using System.IO;


namespace Functionland.FxFiles.Shared.Services.Implementations
{
    public partial class FakeFileService : IFileService
    {
        [AutoInject]
        public IStringLocalizer<AppStrings> StringLocalizer { get; set; } = default!;
        private ConcurrentBag<FsArtifact> _files = new ConcurrentBag<FsArtifact>();
        public TimeSpan? ActionLatency { get; set; }
        public TimeSpan? EnumerationLatency { get; set; }

        public FakeFileService(IEnumerable<FsArtifact> files, TimeSpan? actionLatency = null, TimeSpan? enumerationLatency = null)
        {
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

                var artifactType = GetFsArtifactType(artifact.FullPath);
                if (artifactType != FsArtifactType.File)
                {
                    await CreateFolder(newPath, artifact.Name, cancellationToken, overwrite);
                    foreach (var file in _files)
                    {
                        if (file.FullPath != artifact.FullPath && file.FullPath.StartsWith(artifact.FullPath) && Path.GetExtension(artifact.FullPath) != "")
                        {
                            var insideNewArtifact = CreateArtifact(file.FullPath.Replace(artifact.FullPath, newPath), artifact.ContentHash);
                            _files.Add(insideNewArtifact);
                        }
                        else if (file.FullPath != artifact.FullPath && file.FullPath.StartsWith(artifact.FullPath))
                        {
                            var insideNewFolder = await CreateFolderAsync(file.FullPath.Replace(artifact.FullPath, newPath), artifact.Name, cancellationToken);
                            _files.Add(insideNewFolder);
                        }
                    }

                }
                else
                {
                    if (overwrite)
                        await DeleteArtifactsAsync(new[] { artifact }, cancellationToken);

                    var newArtifact = CreateArtifact(newPath, artifact.ContentHash);
                    _files.Add(newArtifact);
                }

            }
        }

        private static FsArtifactType GetFsArtifactType(string path)
        {
            string[] drives = GetDrives();

            if (drives.Contains(path))
            {
                return FsArtifactType.Drive;
            }

            if (Path.GetExtension("c:\\Folder") == "")
            {
                return FsArtifactType.Folder;
            }
            else
            {
                return FsArtifactType.File;
            }
        }
        private void CheckIfArtifactExist(string newPath)
        {
            if (ArtifacExist(newPath))
                throw new DomainLogicException(StringLocalizer[nameof(AppStrings.ArtifactAlreadyExistsException)]);
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
            if (string.IsNullOrWhiteSpace(stream?.ToString()))
                throw new DomainLogicException(StringLocalizer.GetString(AppStrings.StreamFileIsNull));

            if (string.IsNullOrWhiteSpace(path))
                throw new DomainLogicException(StringLocalizer.GetString(AppStrings.ArtifactPathIsNull, "file"));

            var fileName = Path.GetFileNameWithoutExtension(path);

            if (string.IsNullOrWhiteSpace(fileName))
                throw new DomainLogicException(StringLocalizer.GetString(AppStrings.ArtifactNameIsNull, "file"));

            if (CheckIfNameHasInvalidChars(fileName))
                throw new DomainLogicException(StringLocalizer.GetString(AppStrings.ArtifactNameHasInvalidChars, "file"));

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
            return new FsArtifact(path, Path.GetFileName(path), GetFsArtifactType(path), FsFileProviderType.InternalMemory)
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
                                     let artifact = CreateArtifact(file.path,file.stream.GetHashCode().ToString())
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
                throw new DomainLogicException(StringLocalizer.GetString(AppStrings.ArtifactPathIsNull, "folder"));


            if (string.IsNullOrWhiteSpace(folderName))
                throw new DomainLogicException(StringLocalizer.GetString(AppStrings.ArtifactNameIsNull, "folder"));

            if (CheckIfNameHasInvalidChars(folderName))
                throw new DomainLogicException(StringLocalizer.GetString(AppStrings.ArtifactNameHasInvalidChars, "folder"));


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
            var tempBag = new List<FsArtifact>();
            var finalBag = new ConcurrentBag<FsArtifact>();

            foreach (var artifact in artifacts)
            {

                if (string.IsNullOrWhiteSpace(artifact.FullPath))
                    throw new DomainLogicException(StringLocalizer.GetString(AppStrings.ArtifactPathIsNull, artifact?.ArtifactType.ToString() ?? ""));


                var artifactExist = ArtifacExist(artifact.FullPath);

                if (!artifactExist)
                    throw new DomainLogicException(StringLocalizer.GetString(AppStrings.ArtifactDoseNotExistsException, artifact?.ArtifactType.ToString() ?? "artifact"));


                if (artifact.ArtifactType != FsArtifactType.Drive)
                {
                    await LatencyEnumerationAsync();
                    foreach (var file in _files)
                    {
                        finalBag.Add(file);
                    }
                    while (!finalBag.IsEmpty)
                    {
                        _ = finalBag.TryTake(result: out FsArtifact? currentItem);

                        if (currentItem != null && !string.Equals(currentItem.FullPath, artifact.FullPath, StringComparison.CurrentCultureIgnoreCase))
                        {
                            tempBag.Add(currentItem);
                        }
                    }
                    foreach (var item in tempBag)
                    {
                        if (!item.FullPath.StartsWith(artifact.FullPath))
                        {
                            finalBag.Add(item);
                        }

                    }
                }
                else if (artifact.ArtifactType == FsArtifactType.Drive)
                {
                    throw new DomainLogicException(StringLocalizer[nameof(AppStrings.DriveRemoveFailed)]);
                }
            }

            _files = finalBag;
        }

        public async IAsyncEnumerable<FsArtifact> GetArtifactsAsync(string? path = null, string? searchText = null, CancellationToken? cancellationToken = null)
        {
            IEnumerable<FsArtifact> files = _files;
            if (!string.IsNullOrWhiteSpace(path))
                files = files.Where(
                    f => string.Equals(Path.GetDirectoryName(f.FullPath), path, StringComparison.CurrentCultureIgnoreCase)
                    && !string.Equals(f.FullPath, path, StringComparison.CurrentCultureIgnoreCase));

            if (searchText is not null)
                files = files.Where(f => f.Name.Contains(searchText));

            if (string.IsNullOrWhiteSpace(path))
            {
                string[] drives = GetDrives();
                var artifacts = new List<FsArtifact>();

                foreach (var drive in drives)
                {
                    string driveName = drive;

                    artifacts.Add(
                        new FsArtifact(drive, driveName, FsArtifactType.Drive, FsFileProviderType.InternalMemory));
                }

                foreach (var drive in artifacts)
                {
                    drive.LastModifiedDateTime = Directory.GetLastWriteTime(drive.FullPath);
                    yield return drive;
                }
                yield break;
            }


            foreach (var file in files)
            {
                await LatencyEnumerationAsync();
                yield return file;
            }

        }

        private static string[] GetDrives()
        {
            return new string[] { "C:\\", "D:\\", "E:\\", "F:\\" };
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
            await Task.WhenAll(
                CopyArtifactsAsync(artifacts, destination, overwrite, cancellationToken),
                DeleteArtifactsAsync(artifacts, cancellationToken));
        }

        public async Task RenameFileAsync(string filePath, string newName, CancellationToken? cancellationToken = null)
        {
            foreach (var articat in _files)
            {
                await LatencyEnumerationAsync();
                if (string.Equals(articat.FullPath, filePath, StringComparison.CurrentCultureIgnoreCase))
                {
                    var directoryName = Path.GetDirectoryName(articat.FullPath);
                    articat.FullPath = Path.Combine(directoryName, newName);
                    articat.Name = newName;
                    break;
                }
            }
        }

        public async Task RenameFolderAsync(string folderPath, string newName, CancellationToken? cancellationToken = null)
        {
            var oldFolderName = Path.GetFileName(folderPath.TrimEnd(Path.DirectorySeparatorChar));
            foreach (var artifact in _files)
            {
                await LatencyEnumerationAsync();
                if (string.Equals(artifact.FullPath, folderPath, StringComparison.CurrentCultureIgnoreCase))
                {
                    DirectoryInfo parentDir = Directory.GetParent(folderPath.EndsWith("\\") ? folderPath : string.Concat(folderPath, "\\"));
                    artifact.FullPath = Path.Combine(parentDir.Parent.FullName, newName);
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

            foreach (var path in paths)
            {
                if (string.IsNullOrWhiteSpace(path))
                    throw new DomainLogicException(StringLocalizer.GetString(AppStrings.ArtifactPathIsNull, ""));

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
