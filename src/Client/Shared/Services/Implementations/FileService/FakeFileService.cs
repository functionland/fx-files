﻿using Functionland.FxFiles.Client.Shared.Components.Modal;
using Functionland.FxFiles.Client.Shared.Utils;
using System.Text;
using System.Text.RegularExpressions;

namespace Functionland.FxFiles.Client.Shared.Services.Implementations.FileService
{
    public class FakeFileService : ILocalDeviceFileService, IFulaFileService
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
            var fileName = Path.GetFileName(path);
            string pattern = @"[[a-z]*=[0-9]*[a-z]b]";
            Match match = Regex.Match(fileName, pattern, RegexOptions.None);
            long size = 20;
            if (match.Success)
            {
                string found = match.Value;
                //fileName = fileName.Replace(found, String.Empty);
                size = GetArtifactSize(found);
            }

            return new FsArtifact(path, fileName, FsArtifactType.File, FsFileProviderType.InternalMemory)
            {
                FileExtension = Path.GetExtension(path),
                OriginDevice = originDevice,
                ThumbnailPath = path,
                ContentHash = contentHash,
                LastModifiedDateTime = DateTimeOffset.Now.ToUniversalTime(),
                Size = size
            };
        }

        private static long GetArtifactSize(string sizeStr)
        {

            sizeStr = sizeStr.TrimStart('[').TrimEnd(']').Replace("size=", "");
            var sizeValueStr = Regex.Match(sizeStr, @"\d+").Value;
            var sizeUnit = sizeStr.Replace(sizeValueStr, "");
            long sizeValue = FsArtifactUtils.ConvertToByte(sizeValueStr, sizeUnit);
            return sizeValue;

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

        private async Task<FsArtifact> CreateFolder(string path, string folderName, CancellationToken? cancellationToken)
        {

            var originDevice = $"{Environment.MachineName}-{Environment.UserName}";
            var artifact = new FsArtifact(path, folderName, FsArtifactType.Folder, FsFileProviderType.InternalMemory)
            {
                OriginDevice = originDevice,
                LastModifiedDateTime = DateTimeOffset.Now.ToUniversalTime(),
            };

            _files.Add(artifact);
            return artifact;
        }

        public async Task DeleteArtifactsAsync(IList<FsArtifact> artifacts, Func<ProgressInfo, Task>? onProgress = null, CancellationToken? cancellationToken = null)
        {
            double progressCount = 0;
            var finalBag = new ConcurrentBag<FsArtifact>();
            var excludedPaths = new List<string>();

            foreach (var artifact in artifacts)
            {
                if (onProgress is not null)
                {
                    progressCount = await FsArtifactUtils.HandleProgressBarAsync(artifact.Name, artifacts.Count(), progressCount, onProgress);
                }

                foreach (var file in _files)
                {
                    if (file.FullPath.StartsWith(artifact.FullPath))
                    {
                        excludedPaths.Add(file.FullPath);

                        if (onProgress is not null)
                        {
                            progressCount = await FsArtifactUtils.HandleProgressBarAsync(file.Name, artifacts.Count(), progressCount, onProgress);
                        }
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

        public async IAsyncEnumerable<FsArtifact> GetArtifactsAsync(string? path = null, CancellationToken? cancellationToken = null)
        {
            if (!string.IsNullOrWhiteSpace(path))
                path = path.Replace("/", "\\");
            IEnumerable<FsArtifact> files = _files;
            if (!string.IsNullOrWhiteSpace(path))
                files = files.Where(
                    f => string.Equals(Path.GetDirectoryName(f.FullPath), path, StringComparison.CurrentCultureIgnoreCase)
                    && !string.Equals(f.FullPath, path, StringComparison.CurrentCultureIgnoreCase));

            foreach (var file in files)
            {
                await LatencyEnumerationAsync();
                yield return file;
            }

        }

        public async Task<FsArtifact> GetArtifactAsync(string? path, CancellationToken? cancellationToken = null)
        {
            await LatencyActionAsync();
            return _files.FirstOrDefault(f => f.FullPath == path)!;
        }

        public async Task<Stream> GetFileContentAsync(string filePath, CancellationToken? cancellationToken = null)
        {
            var fileName = Path.GetFileName(filePath);
            string pattern = @"[[a-z]*=[0-9]*[a-z]b]";
            Match match = Regex.Match(fileName, pattern, RegexOptions.None);
            long size = 20;
            if (match.Success)
            {
                string found = match.Value;
                size = GetArtifactSize(found);
            }


            await LatencyActionAsync();
            var sampleText = "Hello streamer!";
            byte[] charArray = new byte[size];
            byte[] byteArray = Encoding.ASCII.GetBytes(sampleText).Concat(charArray).ToArray();

            MemoryStream stream = new(byteArray);
            return stream;

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
                        throw new ArtifactAlreadyExistsException(StringLocalizer.GetString(AppStrings.ArtifactAlreadyExistsException));

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

        public async Task<List<(string Path, bool IsExist)>> CheckPathExistsAsync(IEnumerable<string?> paths, CancellationToken? cancellationToken = null)
        {
            var fsArtifactList = new List<(string Path, bool IsExist)>();

            foreach (var currentPath in paths)
            {
                var path = currentPath.Replace("/", "\\");
                if (string.IsNullOrWhiteSpace(path))
                    throw new ArtifactPathNullException(StringLocalizer.GetString(AppStrings.ArtifactPathIsNull, ""));

                var artifactIsExist = ArtifacExist(path);

                var isExist = false;

                if (artifactIsExist)
                {
                    isExist = true;
                }
                else
                {
                    isExist = false;
                  
                }


                fsArtifactList.Add((path, isExist));
            }

            return fsArtifactList;
        }

        public Task FillArtifactMetaAsync(FsArtifact fsArtifact, CancellationToken? cancellationToken = null)
        {
            //TODO: Fill FsArtifact's data
            return Task.CompletedTask;
        }

        public Task<List<FsArtifactActivity>> GetArtifactActivityHistoryAsync(string path, long? page = null, long? pageSize = null, CancellationToken? cancellationToken = null)
        {
            throw new NotImplementedException();
        }

        public async IAsyncEnumerable<FsArtifact> GetSearchArtifactAsync(DeepSearchFilter? deepSearchFilter, CancellationToken? cancellationToken = null)
        {
            if (deepSearchFilter is null)
                throw new DomainLogicException("Search filter in empty."); // TODO: return proper exception.
            // TODO : Implement deep search 
            IEnumerable<FsArtifact> files = _files;

            if (!string.IsNullOrWhiteSpace(deepSearchFilter.Value.SearchText))
                files = files.Where(f => f.Name.Contains(deepSearchFilter.Value.SearchText));

            foreach (var file in files)
            {
                await LatencyEnumerationAsync();
                yield return file;
            }
        }

        public Task<long> GetArtifactSizeAsync(string path, Action<long>? onProgress, CancellationToken? cancellationToken = null)
        {
            throw new NotImplementedException();
        }

        public string GetShowablePath(string artifactPath)
        {
            return artifactPath;
        }

        public async Task<List<(FsArtifact artifact, Exception exception)>> CopyArtifactsAsync(IList<FsArtifact> artifacts,
                                                                                               string destination,
                                                                                               Func<FsArtifact, Task<bool>>? onShouldOverwrite = null,
                                                                                               Func<ProgressInfo, Task>? onProgress = null,
                                                                                               CancellationToken? cancellationToken = null)
        {
            List<(FsArtifact artifact, Exception exception)> ignoredList = new();

            await CopyAllAsync(artifacts, destination, ignoredList, onShouldOverwrite, onProgress, true, cancellationToken);

            return ignoredList;
        }

        public Task CopyFileAsync(FsArtifact artifact,
                                  string destinationFullPath,
                                  Func<FsArtifact, Task<bool>>? onShouldOverwrite = null,
                                  Func<ProgressInfo, Task>? onProgress = null,
                                  CancellationToken? cancellationToken = null)
        {
            throw new NotImplementedException();
        }

        public async Task<List<(FsArtifact artifact, Exception exception)>> MoveArtifactsAsync(IList<FsArtifact> artifacts,
                                                                                               string destination,
                                                                                               Func<FsArtifact, Task<bool>>? onShouldOverwrite = null,
                                                                                               Func<ProgressInfo, Task>? onProgress = null,
                                                                                               CancellationToken? cancellationToken = null)
        {
            List<(FsArtifact artifact, Exception exception)> ignoredList = new();
            var finalBag = new ConcurrentBag<FsArtifact>();
            var excludedPaths = new List<string>();
            List<FsArtifact> shouldBeRemoved = new();

            await CopyAllAsync(artifacts, destination, ignoredList, onShouldOverwrite, onProgress, true, cancellationToken);

            var pureArtifacts = ignoredList.Select(i => i.artifact).ToList();
            shouldBeRemoved = artifacts.Except(pureArtifacts).ToList();

            foreach (var artifact in shouldBeRemoved)
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

            return ignoredList;
        }
        private async Task CopyAllAsync(IList<FsArtifact> artifacts,
           string destination,
           List<(FsArtifact artifact, Exception exception)> ignoredList,
           Func<FsArtifact, Task<bool>>? onShouldOverwrite = null,
           Func<ProgressInfo, Task>? onProgress = null,
           bool shouldProgress = true,
           CancellationToken? cancellationToken = null)
        {
            double progressCount = 0;

            foreach (var artifact in artifacts)
            {
                if (onProgress is not null && shouldProgress)
                {
                    progressCount = await FsArtifactUtils.HandleProgressBarAsync(artifact.Name, artifacts.Count, progressCount, onProgress);
                }

                if (cancellationToken?.IsCancellationRequested == true) break;

                if (artifact.ArtifactType == FsArtifactType.File)
                {
                    await CopyFile(destination, ignoredList, onShouldOverwrite, cancellationToken, artifact);
                }
                else if (artifact.ArtifactType == FsArtifactType.Folder)
                {
                    CopyFolder(destination, artifact);

                    var newPath = Path.Combine(destination, artifact.Name);

                    var details = _files.Where(f => f.FullPath.StartsWith(artifact.FullPath) && f.FullPath != artifact.FullPath);
                    foreach (var detail in details)
                    {
                        if (detail.ArtifactType == FsArtifactType.File)
                        {
                            await CopyFile(newPath, ignoredList, onShouldOverwrite, cancellationToken, detail);
                        }
                        else
                        {
                            CopyFolder(newPath, artifact);
                        }
                    }

                }

                if (onProgress is not null && shouldProgress)
                {
                    progressCount = await FsArtifactUtils.HandleProgressBarAsync(artifact.Name, artifacts.Count, progressCount, onProgress);
                }
            }
        }

        private void CopyFolder(string destination, FsArtifact artifact)
        {

            var newPath = Path.Combine(destination, Path.GetFileName(artifact.FullPath));

            if (!_files.Any(c => c.FullPath == newPath))
            {
                var newArtifact = new FsArtifact(newPath, artifact.Name, artifact.ArtifactType, artifact.ProviderType)
                {
                    LastModifiedDateTime = artifact.LastModifiedDateTime
                };
                _files.Add(newArtifact);
            }
        }

        private async Task CopyFile(string destination,
                                    List<(FsArtifact artifact, Exception exception)> ignoredList,
                                    Func<FsArtifact, Task<bool>>? onShouldOverwrite,
                                    CancellationToken? cancellationToken,
                                    FsArtifact artifact)
        {
            var newPath = Path.Combine(destination, artifact.Name);

            var destinationInfo = await CheckPathExistsAsync(new List<string>() { newPath }, cancellationToken);
            var shouldCopy = true;

            if (destinationInfo.First().IsExist)
            {
                if (onShouldOverwrite is null)
                {
                    shouldCopy = false;
                    ignoredList.Add((artifact, new ArtifactAlreadyExistsException(artifact, AppStrings.ArtifactAlreadyExistsException)));
                }
                else
                {
                    shouldCopy = await onShouldOverwrite(artifact);
                }
            }

            if (shouldCopy)
            {

                try
                {
                    var newArtifact = new FsArtifact(newPath, artifact.Name, artifact.ArtifactType, artifact.ProviderType)
                    {
                        Size = artifact.Size,
                        LastModifiedDateTime = artifact.LastModifiedDateTime
                    };
                    if (!_files.Any(c => c.FullPath == newArtifact.FullPath))
                        _files.Add(newArtifact);
                }
                catch (Exception exception)
                {
                    ignoredList.Add((artifact, exception));
                }
            }
        }

        public string GetFileName(string path)
        {
            return Path.GetFileName(path);
        }
    }
}
