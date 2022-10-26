using System.Text;
using Functionland.FxFiles.Client.Shared.Components.Modal;
using Functionland.FxFiles.Client.Shared.Extensions;

namespace Functionland.FxFiles.Client.Shared.Services.Implementations
{
    public abstract partial class LocalDeviceFileService : ILocalDeviceFileService
    {
        [AutoInject] public IStringLocalizer<AppStrings> StringLocalizer { get; set; } = default!;

        public abstract Task<FsFileProviderType> GetFsFileProviderTypeAsync(string filePath);

        public virtual async Task CopyArtifactsAsync(IList<FsArtifact> artifacts, string destination, bool overwrite = false, Action<ProgressInfo>? onProgress = null, CancellationToken? cancellationToken = null)
        {
            List<FsArtifact> ignoredList = new();

            await Task.Run(async () =>
            {
                ignoredList = await CopyAllAsync(artifacts, destination, false, overwrite, ignoredList, onProgress, true, cancellationToken);
            });

            if (ignoredList.Any())
            {
                throw new CanNotOperateOnFilesException(StringLocalizer[nameof(AppStrings.CanNotOperateOnFilesException)], ignoredList);
            }
        }

        public virtual async Task<FsArtifact> CreateFileAsync(string path, Stream stream, CancellationToken? cancellationToken = null)
        {
            var lowerCaseFile = AppStrings.File.ToLowerFirstChar();

            if (string.IsNullOrWhiteSpace(stream?.ToString()))
                throw new StreamNullException(StringLocalizer.GetString(AppStrings.StreamFileIsNull));

            if (string.IsNullOrWhiteSpace(path))
                throw new ArtifactPathNullException(StringLocalizer.GetString(AppStrings.ArtifactPathIsNull, lowerCaseFile));

            var fileName = Path.GetFileNameWithoutExtension(path);

            if (string.IsNullOrWhiteSpace(fileName))
                throw new ArtifactNameNullException(StringLocalizer.GetString(AppStrings.ArtifactNameIsNullException));

            if (CheckIfNameHasInvalidChars(fileName))
                throw new ArtifactInvalidNameException(StringLocalizer.GetString(AppStrings.ArtifactNameHasInvalidCharsException));

            if (File.Exists(path))
                throw new ArtifactAlreadyExistsException(StringLocalizer.GetString(AppStrings.ArtifactAlreadyExistsException, lowerCaseFile));

            using FileStream outPutFileStream = new(path, FileMode.Create);
            await stream.CopyToAsync(outPutFileStream);

            var newFsArtifact = new FsArtifact(path, fileName, FsArtifactType.File, await GetFsFileProviderTypeAsync(path))
            {
                FileExtension = Path.GetExtension(path),
                Size = outPutFileStream.Length,
                LastModifiedDateTime = File.GetLastWriteTime(path),
                ParentFullPath = Directory.GetParent(path)?.FullName,
            };

            return newFsArtifact;
        }

        public virtual async Task<List<FsArtifact>> CreateFilesAsync(IEnumerable<(string path, Stream stream)> files, CancellationToken? cancellationToken = null)
        {
            List<FsArtifact> fsArtifacts = new();

            foreach (var (path, stream) in files)
            {
                fsArtifacts.Add(await CreateFileAsync(path, stream, cancellationToken));
            }

            return fsArtifacts;
        }

        public virtual async Task<FsArtifact> CreateFolderAsync(string path, string folderName, CancellationToken? cancellationToken = null)
        {
            var lowerCaseFolder = AppStrings.Folder.ToLowerFirstChar();

            if (string.IsNullOrWhiteSpace(path))
                throw new ArtifactPathNullException(StringLocalizer.GetString(AppStrings.ArtifactPathIsNull, lowerCaseFolder));


            if (string.IsNullOrWhiteSpace(folderName))
                throw new ArtifactNameNullException(StringLocalizer.GetString(AppStrings.ArtifactNameIsNullException));

            if (CheckIfNameHasInvalidChars(folderName))
                throw new ArtifactInvalidNameException(StringLocalizer.GetString(AppStrings.ArtifactNameHasInvalidCharsException));

            var newPath = Path.Combine(path, folderName);

            if (Directory.Exists(newPath))
                throw new ArtifactAlreadyExistsException(StringLocalizer.GetString(AppStrings.ArtifactAlreadyExistsException, lowerCaseFolder));

            Directory.CreateDirectory(newPath);

            var newFsArtifact = new FsArtifact(newPath, folderName, FsArtifactType.Folder, await GetFsFileProviderTypeAsync(newPath))
            {
                ParentFullPath = Directory.GetParent(newPath)?.FullName,
                LastModifiedDateTime = Directory.GetLastWriteTime(newPath)
            };

            return newFsArtifact;
        }

        public virtual async Task DeleteArtifactsAsync(IList<FsArtifact> artifacts, Action<ProgressInfo>? onProgress = null, CancellationToken? cancellationToken = null)
        {
            int? progressCount = null;

            foreach (var artifact in artifacts)
            {
                if (onProgress is not null && progressCount == null)
                {
                    progressCount = HandleProgressBar(artifact.Name, artifacts.Count, progressCount, onProgress);
                }

                if (cancellationToken?.IsCancellationRequested == true)
                    break;

                DeleteArtifactAsync(artifact);

                if (onProgress is not null)
                {
                    progressCount = HandleProgressBar(artifact.Name, artifacts.Count, progressCount, onProgress);
                }
            }
        }

        private void DeleteArtifactAsync(FsArtifact artifact, CancellationToken? cancellationToken = null)
        {
            var lowerCaseArtifact = AppStrings.Artifact.ToLowerFirstChar();

            if (string.IsNullOrWhiteSpace(artifact.FullPath))
                throw new ArtifactPathNullException(StringLocalizer.GetString(AppStrings.ArtifactPathIsNull, artifact?.ArtifactType.ToString() ?? ""));

            var isDirectoryExist = Directory.Exists(artifact.FullPath);
            var isFileExist = File.Exists(artifact.FullPath);

            if ((artifact.ArtifactType == FsArtifactType.Folder && !isDirectoryExist) ||
                (artifact.ArtifactType == FsArtifactType.File && !isFileExist))
                throw new ArtifactDoseNotExistsException(StringLocalizer.GetString(AppStrings.ArtifactDoseNotExistsException, artifact?.ArtifactType.ToString() ?? lowerCaseArtifact));

            if (artifact.ArtifactType == FsArtifactType.Folder)
            {
                Directory.Delete(artifact.FullPath, true);
            }
            else if (artifact.ArtifactType == FsArtifactType.File)
            {
                File.Delete(artifact.FullPath);
            }
            else if (artifact.ArtifactType == FsArtifactType.Drive)
            {
                throw new CanNotModifyOrDeleteDriveException(StringLocalizer[nameof(AppStrings.DriveRemoveFailed)]);
            }
        }

        public virtual async IAsyncEnumerable<FsArtifact> GetArtifactsAsync(string? path = null, CancellationToken? cancellationToken = null)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                await foreach (var item in GetChildArtifactsAsync(path, cancellationToken))
                {
                    if (cancellationToken?.IsCancellationRequested == true) yield break;
                    yield return item;
                }

                yield break;
            }

            await foreach (var item in GetChildArtifactsAsync(path, cancellationToken))
            {
                if (cancellationToken?.IsCancellationRequested == true) yield break;
                yield return item;
            }
        }

        public virtual async Task<FsArtifact> GetArtifactAsync(string? path, CancellationToken? cancellationToken = null)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArtifactPathNullException(StringLocalizer.GetString(AppStrings.ArtifactPathIsNull, ""));

            var fsArtifactType = await GetFsArtifactTypeAsync(path);

            var fsArtifact = new FsArtifact(path, Path.GetFileName(path), fsArtifactType.Value, await GetFsFileProviderTypeAsync(path))
            {
                FileExtension = Path.GetExtension(path),
                ParentFullPath = Directory.GetParent(path)?.FullName
            };


            if (fsArtifactType == FsArtifactType.File)
            {
                fsArtifact.LastModifiedDateTime = File.GetLastWriteTime(path);
            }
            else if (fsArtifactType == FsArtifactType.Folder)
            {
                fsArtifact.LastModifiedDateTime = Directory.GetLastWriteTime(path);
            }
            else if (fsArtifactType == FsArtifactType.Drive)
            {
                var drives = await GetDrivesAsync();
                fsArtifact = drives.FirstOrDefault(drives => drives.FullPath == path)!;
            }

            return fsArtifact;
        }

        public virtual async Task<Stream> GetFileContentAsync(string filePath, CancellationToken? cancellationToken = null)
        {
            var lowerCaseFile = AppStrings.File.ToLowerFirstChar();

            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArtifactPathNullException(StringLocalizer.GetString(AppStrings.ArtifactPathIsNull, lowerCaseFile));

            var streamReader = new StreamReader(filePath);
            return streamReader.BaseStream;
        }

        public virtual async Task MoveArtifactsAsync(IList<FsArtifact> artifacts, string destination, bool overwrite = false, Action<ProgressInfo>? onProgress = null, CancellationToken? cancellationToken = null)
        {
            List<FsArtifact> ignoredList = new();

            await Task.Run(async () =>
            {
                ignoredList = await CopyAllAsync(artifacts, destination, true, overwrite, ignoredList, onProgress, true, cancellationToken);
            });

            if (ignoredList.Any())
            {
                throw new CanNotOperateOnFilesException(StringLocalizer[nameof(AppStrings.CanNotOperateOnFilesException)], ignoredList);
            }
        }

        public virtual async Task RenameFileAsync(string filePath, string newName, CancellationToken? cancellationToken = null)
        {
            var lowerCaseFile = AppStrings.File.ToLowerFirstChar();

            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArtifactPathNullException(StringLocalizer.GetString(AppStrings.ArtifactPathIsNull, lowerCaseFile));

            var artifactType = GetFsArtifactTypeAsync(filePath);

            if (string.IsNullOrWhiteSpace(newName))
                throw new ArtifactNameNullException(StringLocalizer.GetString(AppStrings.ArtifactNameIsNullException));

            if (cancellationToken?.IsCancellationRequested == true) return;

            if (CheckIfNameHasInvalidChars(newName))
                throw new ArtifactInvalidNameException(StringLocalizer.GetString(AppStrings.ArtifactNameHasInvalidCharsException));

            var isExistOld = File.Exists(filePath);

            if (!isExistOld)
                throw new ArtifactDoseNotExistsException(StringLocalizer.GetString(AppStrings.ArtifactNameHasInvalidCharsException));

            await Task.Run(() =>
            {

                var directory = Path.GetDirectoryName(filePath);
                var isExtentionExsit = Path.HasExtension(newName);
                var newFileName = isExtentionExsit ? newName : Path.ChangeExtension(newName, Path.GetExtension(filePath));
                var newPath = Path.Combine(directory, newFileName);

                var isFileExist = File.Exists(newPath);

                if (isFileExist)
                    throw new ArtifactAlreadyExistsException(StringLocalizer.GetString(AppStrings.ArtifactAlreadyExistsException, lowerCaseFile));

                File.Move(filePath, newPath);
            });
        }

        public virtual async Task RenameFolderAsync(string folderPath, string newName, CancellationToken? cancellationToken = null)
        {
            var lowerCaseFolder = AppStrings.Folder.ToLowerFirstChar();

            if (string.IsNullOrWhiteSpace(folderPath))
                throw new ArtifactPathNullException(StringLocalizer.GetString(AppStrings.ArtifactPathIsNull, lowerCaseFolder));

            var artifactType = GetFsArtifactTypeAsync(folderPath);

            if (artifactType is null)
                throw new ArtifactTypeNullException(StringLocalizer[nameof(AppStrings.ArtifactTypeIsNull)]);

            if (string.IsNullOrWhiteSpace(newName))
                throw new ArtifactNameNullException(StringLocalizer.GetString(AppStrings.ArtifactNameIsNullException));

            if (cancellationToken?.IsCancellationRequested == true) return;

            if (CheckIfNameHasInvalidChars(newName))
                throw new ArtifactInvalidNameException(StringLocalizer.GetString(AppStrings.ArtifactNameHasInvalidCharsException));

            var isExistOld = Directory.Exists(folderPath);

            if (!isExistOld)
                throw new ArtifactDoseNotExistsException(StringLocalizer.GetString(AppStrings.ArtifactDoseNotExistsException, lowerCaseFolder));

            var fsArtifactType = await GetFsArtifactTypeAsync(folderPath);

            if (fsArtifactType is FsArtifactType.Drive)
                throw new CanNotModifyOrDeleteDriveException(StringLocalizer.GetString(AppStrings.DriveRenameFailed));

            await Task.Run(() =>
            {
                var oldName = Path.GetFileName(folderPath);
                var newPath = Path.Combine(Path.GetDirectoryName(folderPath), newName);

                var isExist = Directory.Exists(newPath);

                if (isExist)
                    throw new ArtifactAlreadyExistsException(StringLocalizer.GetString(AppStrings.ArtifactAlreadyExistsException, lowerCaseFolder));

                Directory.Move(folderPath, newPath);
            });
        }

        private async Task<List<FsArtifact>> CopyAllAsync(
            IList<FsArtifact> artifacts,
            string destination,
            bool mustDeleteSource = false,
            bool overwrite = false,
            List<FsArtifact>? ignoredList = null,
            Action<ProgressInfo>? onProgress = null,
            bool shouldProgress = true,
            CancellationToken? cancellationToken = null)
        {
            int? progressCount = null;

            foreach (var artifact in artifacts)
            {
                if (onProgress is not null && shouldProgress && progressCount == null)
                {
                    progressCount = HandleProgressBar(artifact.Name, artifacts.Count, progressCount, onProgress);
                }

                if (cancellationToken?.IsCancellationRequested == true) break;

                if (artifact.ArtifactType == FsArtifactType.File)
                {
                    var fileInfo = new FileInfo(artifact.FullPath);
                    var destinationInfo = new FileInfo(Path.Combine(destination, Path.GetFileName(artifact.FullPath)));

                    if (fileInfo.FullName == destinationInfo.FullName)
                        throw new SameDestinationFileException(StringLocalizer.GetString(AppStrings.SameDestinationFileException));

                    if (!overwrite && destinationInfo.Exists)
                    {
                        ignoredList.Add(artifact);
                    }
                    else
                    {
                        var destinationDirectory = new DirectoryInfo(Path.GetFullPath(destination));

                        if (!destinationDirectory.Exists)
                        {
                            destinationDirectory.Create();
                        }

                        fileInfo.CopyTo(destinationInfo.FullName, true);

                        if (mustDeleteSource)
                        {
                            DeleteArtifactAsync(artifact, cancellationToken);
                        }
                    }
                }
                else if (artifact.ArtifactType == FsArtifactType.Folder)
                {
                    var directoryInfo = new DirectoryInfo(artifact.FullPath);
                    var destinationInfo = new DirectoryInfo(Path.Combine(destination, Path.GetFileName(artifact.FullPath)));

                    if (directoryInfo.FullName == destinationInfo.FullName)
                        throw new SameDestinationFolderException(StringLocalizer.GetString(AppStrings.SameDestinationFolderException));

                    if (!destinationInfo.Exists)
                    {
                        destinationInfo.Create();
                    }

                    var children = new List<FsArtifact>();

                    var directoryFiles = directoryInfo.GetFiles();

                    foreach (var file in directoryFiles)
                    {
                        if (cancellationToken?.IsCancellationRequested == true) break;

                        var providerType = await GetFsFileProviderTypeAsync(file.FullName);

                        children.Add(new FsArtifact(file.FullName, file.Name, FsArtifactType.File, providerType)
                        {
                            FileExtension = file.Extension,
                            LastModifiedDateTime = file.LastWriteTime,
                            ParentFullPath = Directory.GetParent(file.FullName)?.FullName,
                            Size = file.Length
                        });
                    }

                    var directorySubDirectories = directoryInfo.GetDirectories();

                    foreach (var subDirectory in directorySubDirectories)
                    {
                        if (cancellationToken?.IsCancellationRequested == true) break;

                        var providerType = await GetFsFileProviderTypeAsync(subDirectory.FullName);

                        children.Add(new FsArtifact(subDirectory.FullName, subDirectory.Name, FsArtifactType.Folder, providerType)
                        {
                            LastModifiedDateTime = subDirectory.LastWriteTime,
                            ParentFullPath = Directory.GetParent(subDirectory.FullName)?.FullName,
                        });
                    }

                    var childIgnoredList = await CopyAllAsync(children, destinationInfo.FullName,
                        mustDeleteSource, overwrite, ignoredList, onProgress, false, cancellationToken);

                    if (!childIgnoredList.Any() && mustDeleteSource)
                    {
                        DeleteArtifactAsync(artifact, cancellationToken);
                    }
                }

                if (onProgress is not null && shouldProgress)
                {
                    progressCount = HandleProgressBar(artifact.Name, artifacts.Count(), progressCount, onProgress);
                }
            }

            return ignoredList;
        }

        public virtual async Task<FsArtifactType?> GetFsArtifactTypeAsync(string path)
        {
            var artifactIsFile = File.Exists(path);
            if (artifactIsFile)
            {
                return FsArtifactType.File;
            }

            var drives = await GetDrivesAsync();
            if (drives.Any(drive => drive.FullPath == path))
            {
                return FsArtifactType.Drive;
            }

            var artifactIsDirectory = Directory.Exists(path);
            if (artifactIsDirectory)
            {
                return FsArtifactType.Folder;
            }

            return null;
        }

        public virtual async Task<List<FsArtifact>> GetDrivesAsync()
        {
            var drives = Directory.GetLogicalDrives();
            var artifacts = new List<FsArtifact>();

            foreach (var drive in drives)
            {
                var driveInfo = new DriveInfo(drive);

                if (!driveInfo.IsReady) continue;

                var lable = driveInfo.VolumeLabel;
                var drivePath = drive.TrimEnd(Path.DirectorySeparatorChar);
                var driveName = !string.IsNullOrWhiteSpace(lable) ? $"{lable} ({drivePath})" : drivePath;

                artifacts.Add(
                    new FsArtifact(drive, driveName, FsArtifactType.Drive, await GetFsFileProviderTypeAsync(drive)));
            }

            return artifacts;
        }

        public virtual async Task<List<FsArtifactChanges>> CheckPathExistsAsync(List<string?> paths, CancellationToken? cancellationToken = null)
        {
            var fsArtifactList = new List<FsArtifactChanges>();

            foreach (var path in paths)
            {
                if (string.IsNullOrWhiteSpace(path))
                    throw new ArtifactPathNullException(StringLocalizer.GetString(AppStrings.ArtifactPathIsNull, ""));

                var artifactIsFile = File.Exists(path);
                var artifactIsDirectory = Directory.Exists(path);

                var fsArtifact = new FsArtifactChanges()
                {
                    ArtifactFullPath = path,
                };

                if (artifactIsFile)
                {
                    fsArtifact.IsPathExist = true;
                }
                else if (artifactIsDirectory)
                {
                    fsArtifact.IsPathExist = true;
                }
                else
                {
                    fsArtifact.IsPathExist = false;
                    fsArtifact.FsArtifactChangesType = FsArtifactChangesType.Delete;
                }

                if (artifactIsFile)
                {
                    fsArtifact.LastModifiedDateTime = File.GetLastWriteTime(path);
                }
                else if (artifactIsDirectory)
                {
                    fsArtifact.LastModifiedDateTime = Directory.GetLastWriteTime(path);
                }

                fsArtifactList.Add(fsArtifact);
            }

            return fsArtifactList;
        }

        private static bool CheckIfNameHasInvalidChars(string name)
        {
            var invalidChars = Path.GetInvalidFileNameChars();

            foreach (var invalid in invalidChars)
                if (name.Contains(invalid)) return true;
            return false;
        }

        private async IAsyncEnumerable<FsArtifact> GetChildArtifactsAsync(string? path = null, CancellationToken? cancellationToken = null)
        {
            var lowerCaseArtifact = AppStrings.Artifact.ToLowerFirstChar();

            if (string.IsNullOrWhiteSpace(path))
            {
                var drives = await GetDrivesAsync();

                foreach (var drive in drives)
                {
                    if (cancellationToken?.IsCancellationRequested == true) yield break;
                    drive.LastModifiedDateTime = Directory.GetLastWriteTime(drive.FullPath);
                    yield return drive;
                }
                yield break;
            }

            var fsArtifactType = await GetFsArtifactTypeAsync(path);

            if (fsArtifactType is null)
                throw new ArtifactDoseNotExistsException(StringLocalizer.GetString(AppStrings.ArtifactDoseNotExistsException, fsArtifactType?.ToString() ?? lowerCaseArtifact));

            if (fsArtifactType is FsArtifactType.Folder or FsArtifactType.Drive)
            {
                string[] files = Array.Empty<string>();
                string[] folders = Array.Empty<string>();

                try
                {
                    files = Directory.GetFiles(path);
                    folders = Directory.GetDirectories(path);
                }
                catch (UnauthorizedAccessException)
                {
                    throw new ArtifactUnauthorizedAccessException(StringLocalizer.GetString(AppStrings.ArtifactUnauthorizedAccessException));
                }
                catch { }

                foreach (var folder in folders)
                {
                    if (cancellationToken?.IsCancellationRequested == true) yield break;
                    var directoryInfo = new DirectoryInfo(folder);

                    if (directoryInfo.Attributes.HasFlag(FileAttributes.Hidden) ||
                        directoryInfo.Attributes.HasFlag(FileAttributes.System) ||
                        directoryInfo.Attributes.HasFlag(FileAttributes.Temporary)) continue;

                    var providerType = await GetFsFileProviderTypeAsync(folder);

                    yield return new FsArtifact(folder, Path.GetFileName(folder), FsArtifactType.Folder, providerType)
                    {
                        ParentFullPath = Directory.GetParent(folder)?.FullName,
                        LastModifiedDateTime = Directory.GetLastWriteTime(folder)
                    };
                }

                foreach (var file in files)
                {
                    if (cancellationToken?.IsCancellationRequested == true) yield break;
                    var fileinfo = new FileInfo(file);

                    if (fileinfo.Attributes.HasFlag(FileAttributes.Hidden) ||
                        fileinfo.Attributes.HasFlag(FileAttributes.System) ||
                        fileinfo.Attributes.HasFlag(FileAttributes.Temporary)) continue;

                    var providerType = await GetFsFileProviderTypeAsync(file);

                    yield return new FsArtifact(file, Path.GetFileName(file), FsArtifactType.File, providerType)
                    {
                        ParentFullPath = Directory.GetParent(file)?.FullName,
                        LastModifiedDateTime = File.GetLastWriteTime(file),
                        FileExtension = Path.GetExtension(file),
                        Size = fileinfo.Length
                    };
                }
            }
            else
            {
                var fileinfo = new FileInfo(path);
                yield return new FsArtifact(path, Path.GetFileName(path), FsArtifactType.File, await GetFsFileProviderTypeAsync(path))
                {
                    ParentFullPath = Directory.GetParent(path)?.FullName,
                    LastModifiedDateTime = File.GetLastWriteTime(path),
                    FileExtension = Path.GetExtension(path),
                    Size = fileinfo.Length
                };
            }
        }

        private async IAsyncEnumerable<FsArtifact> GetAllFileAndFoldersAsync(string path, DeepSearchFilter? deepSearchFilter = null, CancellationToken? cancellationToken = null)
        {
            var files = new List<(string fullPath, FileInfo fileInfo)>();
            var folders = new List<(string fullPath, DirectoryInfo directoryInfo)>();

            if (cancellationToken?.IsCancellationRequested == true) yield break;

            try
            {
                files = Directory
                    .GetFiles(path)
                    .Select(c => (c, new FileInfo(c)))
                    .Where(c =>
                            !c.Item2.Attributes.HasFlag(FileAttributes.Hidden) &&
                            !c.Item2.Attributes.HasFlag(FileAttributes.System) &&
                            !c.Item2.Attributes.HasFlag(FileAttributes.Temporary) &&
                             c.Item2.Name.ToLower().Contains(deepSearchFilter?.SearchText.ToLower())
                        )
                    .ToList();

                if (cancellationToken?.IsCancellationRequested == true) yield break;


                folders = Directory
                        .GetDirectories(path)
                        .Select(c => (c, new DirectoryInfo(c)))
                        .Where(c =>
                                !c.Item2.Attributes.HasFlag(FileAttributes.Hidden) &&
                                !c.Item2.Attributes.HasFlag(FileAttributes.System) &&
                                !c.Item2.Attributes.HasFlag(FileAttributes.Temporary)
                            )
                        .ToList();

                if (cancellationToken?.IsCancellationRequested == true) yield break;

            }
            catch { }

            var filesCount = files.Count;

            for (int i = 0; i < filesCount; i++)
            {
                var (fullPath, fileInfo) = files[i];
                if (cancellationToken?.IsCancellationRequested == true) yield break;

                var providerType = await GetFsFileProviderTypeAsync(fullPath);

                yield return new FsArtifact(fullPath, Path.GetFileName(fullPath), FsArtifactType.File, providerType)
                {
                    ParentFullPath = Directory.GetParent(fullPath)?.FullName,
                    LastModifiedDateTime = File.GetLastWriteTime(fullPath),
                    FileExtension = Path.GetExtension(fullPath),
                    Size = fileInfo.Length
                };
            }

            files.Clear();

            var foldersCount = folders.Count;

            for (int i = 0; i < foldersCount; i++)
            {
                var (fullPath, directoryInfo) = folders[i];
                if (cancellationToken?.IsCancellationRequested == true) yield break;

                var providerType = await GetFsFileProviderTypeAsync(fullPath);

                if (directoryInfo.Name.ToLower().Contains(deepSearchFilter?.SearchText.ToLower()))
                {
                    yield return new FsArtifact(fullPath, Path.GetFileName(fullPath), FsArtifactType.Folder, providerType)
                    {
                        ParentFullPath = Directory.GetParent(fullPath)?.FullName,
                        LastModifiedDateTime = Directory.GetLastWriteTime(fullPath)
                    };
                }

                await foreach (var item in GetAllFileAndFoldersAsync(fullPath, deepSearchFilter, cancellationToken))
                {
                    if (cancellationToken?.IsCancellationRequested == true) yield break;
                    yield return item;
                }
            }

            folders.Clear();
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

        private int HandleProgressBar(string artifactName, int totalCount, int? progressCount, Action<ProgressInfo> onProgress)
        {
            if (progressCount != null)
            {
                progressCount++;
            }
            else
            {
                progressCount = 0;
            }

            var subText = $"{progressCount} of {totalCount}";

            onProgress(new ProgressInfo
            {
                CurrentText = artifactName,
                CurrentSubText = subText,
                CurrentValue = progressCount,
                MaxValue = totalCount
            });

            return progressCount.Value;
        }

        public virtual async IAsyncEnumerable<FsArtifact> GetSearchArtifactAsync(DeepSearchFilter? deepSearchFilter, CancellationToken? cancellationToken = null)
        {
            var drives = await GetDrivesAsync();

            foreach (var drive in drives)
            {
                if (cancellationToken?.IsCancellationRequested == true) yield break;
                await foreach (var item in GetAllFileAndFoldersAsync(drive.FullPath, deepSearchFilter, cancellationToken))
                {
                    if (cancellationToken?.IsCancellationRequested == true) yield break;
                    yield return item;
                }
            }
            yield break;
        }
    }
}
