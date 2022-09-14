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

        public async Task CopyArtifactsAsync(FsArtifact[] artifacts, string destination, CancellationToken? cancellationToken = null)
        {
            foreach (var artifact in artifacts)
            {
                await LatencyEnumerationAsync();
                var newPath = Path.Combine(destination, artifact.Name);
                CheckIfArtifactExist(newPath);

                var artifactType = GetFsArtifactType(artifact.FullPath);
                if (artifactType != FsArtifactType.File)
                {
                    await CreateFolderAsync(newPath, artifact.Name, cancellationToken);
                    foreach (var file in _files)
                    {
                        if (file.FullPath != artifact.FullPath && file.FullPath.StartsWith(artifact.FullPath))
                        {
                            var insideNewArtifact = CreateArtifact(file.FullPath.Replace(artifact.FullPath, newPath), artifact.ContentHash);
                            _files.Add(insideNewArtifact);
                        }
                    }

                }
                else
                {
                    var newArtifact = CreateArtifact(newPath, artifact.ContentHash);
                    _files.Add(newArtifact);
                }

            }
        }

        private static FsArtifactType GetFsArtifactType(string path)
        {
            string[] drives = Directory.GetLogicalDrives();

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
                throw new DomainLogicException(StringLocalizer[nameof(AppStrings.FileAlreadyExistsException)]);
        }

        private bool ArtifacExist(string newPath)
        {
            StringComparer comparer = StringComparer.OrdinalIgnoreCase;
            return _files.Any(f => comparer.Compare(f.FullPath, newPath) == 0);
        }

        public async Task<FsArtifact> CreateFileAsync(string path, Stream stream, CancellationToken? cancellationToken = null)
        {
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
            return new FsArtifact
            {
                Name = Path.GetFileName(path),
                FullPath = path,
                FileExtension = Path.GetExtension(path),
                OriginDevice = originDevice,
                ThumbnailPath = path,
                ContentHash = contentHash,
                ProviderType = FsFileProviderType.InternalMemory,
                LastModifiedDateTime = DateTimeOffset.Now.ToUniversalTime(),
                ArtifactType = GetFsArtifactType(path)
            };
        }

        public async Task<List<FsArtifact>> CreateFilesAsync(IEnumerable<(string path, Stream stream)> files, CancellationToken? cancellationToken = null)
        {
            var originDevice = $"{Environment.MachineName}-{Environment.UserName}";
            var addedFiles = new List<FsArtifact>();
            foreach (var artifact in from file in files
                                     let artifact = new FsArtifact
                                     {
                                         Name = Path.GetFileName(file.path),
                                         FullPath = file.path,
                                         FileExtension = Path.GetExtension(file.path),
                                         OriginDevice = originDevice,
                                         ThumbnailPath = file.path,
                                         ContentHash = file.stream.GetHashCode().ToString(),
                                         ProviderType = FsFileProviderType.InternalMemory,
                                         LastModifiedDateTime = DateTimeOffset.Now.ToUniversalTime()
                                     }
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
            await LatencyActionAsync();
            if (path is null) throw new Exception();
            var originDevice = $"{Environment.MachineName}-{Environment.UserName}";
            var finalPath = Path.Combine(path, folderName);
            CheckIfArtifactExist(finalPath);

            var artifact = new FsArtifact
            {
                Name = folderName,
                FullPath = finalPath,
                OriginDevice = originDevice,
                ProviderType = FsFileProviderType.InternalMemory,
                LastModifiedDateTime = DateTimeOffset.Now.ToUniversalTime(),
                ArtifactType = GetFsArtifactType(finalPath)
            };
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
                    throw new DomainLogicException(StringLocalizer.GetString(AppStrings.ArtifactPathIsNull, artifact?.ArtifactType?.ToString() ?? ""));

                if (artifact.ArtifactType == null)
                    throw new DomainLogicException(StringLocalizer[nameof(AppStrings.ArtifactTypeIsNull)]);

                if(artifact.ArtifactType != FsArtifactType.Drive)
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
            if (path is not null)
                files = files.Where(f => f.FullPath.StartsWith(path));

            if (searchText is not null)
                files = files.Where(f => f.Name.Contains(searchText));

            foreach (var file in files)
            {
                await LatencyEnumerationAsync();
                yield return file;
            }

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

        public async Task MoveArtifactsAsync(FsArtifact[] artifacts, string destination, CancellationToken? cancellationToken = null)
        {
            await Task.WhenAll(
                CopyArtifactsAsync(artifacts, destination, cancellationToken),
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
    }
}
