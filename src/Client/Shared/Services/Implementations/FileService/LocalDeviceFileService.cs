using System.Net.Http.Headers;
using System.Text;

using Functionland.FxFiles.Client.Shared.Components.Modal;
using Functionland.FxFiles.Client.Shared.Extensions;
using Functionland.FxFiles.Client.Shared.Utils;

namespace Functionland.FxFiles.Client.Shared.Services.Implementations
{
    public abstract partial class LocalDeviceFileService : ILocalDeviceFileService
    {
        [AutoInject] public IStringLocalizer<AppStrings> StringLocalizer { get; set; } = default!;

        public abstract FsFileProviderType GetFsFileProviderType(string filePath);

        public virtual async Task<List<(FsArtifact artifact, Exception exception)>> CopyArtifactsAsync(IList<FsArtifact> artifacts,
            string destination,
            Func<FsArtifact, Task<bool>>? onShouldOverwrite = null,
            Func<ProgressInfo, Task>? onProgress = null,
            CancellationToken? cancellationToken = null)
        {
            List<(FsArtifact artifact, Exception exception)> ignoredList = new();

            await CopyAllAsync(artifacts, destination, ignoredList, onShouldOverwrite, onProgress, true, cancellationToken);

            return ignoredList;
        }

        public virtual async Task CopyFileAsync(FsArtifact artifact,
            string destinationFullPath,
            Func<FsArtifact, Task<bool>>? onShouldOverwrite = null,
            Func<ProgressInfo, Task>? onProgress = null,
            CancellationToken? cancellationToken = null)
        {
            File.Copy(artifact.FullPath, destinationFullPath);
        }

        public virtual async Task<FsArtifact> CreateFileAsync(string path, Stream stream, CancellationToken? cancellationToken = null)
        {
            var lowerCaseFile = StringLocalizer[nameof(AppStrings.File)].Value.ToLowerFirstChar();

            if (string.IsNullOrWhiteSpace(stream?.ToString()))
                throw new StreamNullException(StringLocalizer.GetString(AppStrings.StreamFileIsNull));

            if (string.IsNullOrWhiteSpace(path))
                throw new ArtifactPathNullException(StringLocalizer.GetString(AppStrings.ArtifactPathIsNull, lowerCaseFile));

            var fileName = Path.GetFileNameWithoutExtension(path);

            if (NameHasInvalidCharacter(fileName))
                throw new ArtifactInvalidNameException(StringLocalizer.GetString(AppStrings.ArtifactNameHasInvalidCharsException));

            if (string.IsNullOrWhiteSpace(fileName))
                throw new ArtifactNameNullException(StringLocalizer.GetString(AppStrings.ArtifactNameIsNullException));

            if (CheckIfNameHasInvalidChars(fileName))
                throw new ArtifactInvalidNameException(StringLocalizer.GetString(AppStrings.ArtifactNameHasInvalidCharsException));

            if (File.Exists(path))
                throw new ArtifactAlreadyExistsException(StringLocalizer.GetString(AppStrings.ArtifactAlreadyExistsException));

            await LocalStorageCreateFile(path, stream);

            var providerType = GetFsFileProviderType(path);
            var newFsArtifact = new FsArtifact(path, fileName, FsArtifactType.File, providerType)
            {
                FileExtension = Path.GetExtension(path),
                Size = stream.Length,
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
            var lowerCaseFolder = StringLocalizer[nameof(AppStrings.Folder)].Value.ToLowerFirstChar();

            if (string.IsNullOrWhiteSpace(path))
                throw new ArtifactPathNullException(StringLocalizer.GetString(AppStrings.ArtifactPathIsNull, lowerCaseFolder));

            if (NameHasInvalidCharacter(folderName))
                throw new ArtifactInvalidNameException(StringLocalizer.GetString(AppStrings.ArtifactNameHasInvalidCharsException));

            if (string.IsNullOrWhiteSpace(folderName))
                throw new ArtifactNameNullException(StringLocalizer.GetString(AppStrings.ArtifactNameIsNullException));

            if (CheckIfNameHasInvalidChars(folderName))
                throw new ArtifactInvalidNameException(StringLocalizer.GetString(AppStrings.ArtifactNameHasInvalidCharsException));

            var newPath = Path.Combine(path, folderName);

            if (Directory.Exists(newPath))
                throw new ArtifactAlreadyExistsException(StringLocalizer.GetString(AppStrings.ArtifactAlreadyExistsException));

            LocalStorageCreateDirectory(newPath);

            var providerType = GetFsFileProviderType(newPath);
            var newFsArtifact = new FsArtifact(newPath, folderName, FsArtifactType.Folder, providerType)
            {
                ParentFullPath = Directory.GetParent(newPath)?.FullName,
                LastModifiedDateTime = Directory.GetLastWriteTime(newPath)
            };

            return newFsArtifact;
        }

        public virtual async Task DeleteArtifactsAsync(IList<FsArtifact> artifacts, Func<ProgressInfo, Task>? onProgress = null, CancellationToken? cancellationToken = null)
        {
            double progressCount = 0;

            foreach (var artifact in artifacts)
            {
                if (onProgress is not null)
                {
                    progressCount = await FsArtifactUtils.HandleProgressBarAsync(artifact.Name, artifacts.Count, progressCount, onProgress);
                }

                if (cancellationToken?.IsCancellationRequested == true)
                    break;

                DeleteArtifactAsync(artifact);

                if (onProgress is not null)
                {
                    progressCount = await FsArtifactUtils.HandleProgressBarAsync(artifact.Name, artifacts.Count, progressCount, onProgress);
                }
            }
        }

        private void DeleteArtifactAsync(FsArtifact artifact, CancellationToken? cancellationToken = null)
        {
            var lowerCaseArtifact = StringLocalizer[nameof(AppStrings.Artifact)].Value.ToLowerFirstChar();

            if (string.IsNullOrWhiteSpace(artifact.FullPath))
                throw new ArtifactPathNullException(StringLocalizer.GetString(AppStrings.ArtifactPathIsNull, artifact?.ArtifactType.ToString() ?? ""));

            var isDirectoryExist = Directory.Exists(artifact.FullPath);
            var isFileExist = File.Exists(artifact.FullPath);

            if ((artifact.ArtifactType == FsArtifactType.Folder && !isDirectoryExist) ||
                (artifact.ArtifactType == FsArtifactType.File && !isFileExist))
                throw new ArtifactDoseNotExistsException(StringLocalizer.GetString(AppStrings.ArtifactDoseNotExistsException, artifact?.ArtifactType.ToString() ?? lowerCaseArtifact));

            if (artifact.ArtifactType == FsArtifactType.Folder)
            {
                LocalStorageDeleteDirectory(artifact.FullPath);
            }
            else if (artifact.ArtifactType == FsArtifactType.File)
            {
                LocalStorageDeleteFile(artifact.FullPath);
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

            var fsArtifactType = GetFsArtifactType(path);

            var providerType = GetFsFileProviderType(path);
            var fsArtifact = new FsArtifact(path, Path.GetFileName(path), fsArtifactType.Value, providerType)
            {
                //ToDo: FileExtension should be exclusive to artifacts of type File, not here which is filled for all type.
                FileExtension = Path.GetExtension(path),
                ParentFullPath = Directory.GetParent(path)?.FullName
            };


            if (fsArtifactType == FsArtifactType.File)
            {
                var fileInfo = new FileInfo(path);
                fsArtifact.Size = fileInfo.Length;
                fsArtifact.LastModifiedDateTime = File.GetLastWriteTime(path);
            }
            else if (fsArtifactType == FsArtifactType.Folder)
            {
                fsArtifact.LastModifiedDateTime = Directory.GetLastWriteTime(path);
            }
            else if (fsArtifactType == FsArtifactType.Drive)
            {
                var drives = GetDrives();
                fsArtifact = drives.FirstOrDefault(drives => drives.FullPath == path)!;
            }

            return fsArtifact;
        }

        public virtual async Task<Stream> GetFileContentAsync(string filePath, CancellationToken? cancellationToken = null)
        {
            var lowerCaseFile = StringLocalizer[nameof(AppStrings.File)].Value.ToLowerFirstChar();

            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArtifactPathNullException(StringLocalizer.GetString(AppStrings.ArtifactPathIsNull, lowerCaseFile));

            var streamReader = new StreamReader(filePath);
            return streamReader.BaseStream;
        }

        public virtual async Task<List<(FsArtifact artifact, Exception exception)>> MoveArtifactsAsync(IList<FsArtifact> artifacts,
            string destination,
            Func<FsArtifact, Task<bool>>? onShouldOverwrite = null,
            Func<ProgressInfo, Task>? onProgress = null,
            CancellationToken? cancellationToken = null)
        {
            List<(FsArtifact artifact, Exception exception)>? ignoredList = new();

            await MoveAllAsync(artifacts, destination, onShouldOverwrite, ignoredList, onProgress, true, cancellationToken);

            return ignoredList;
        }

        public virtual async Task RenameFileAsync(string filePath, string newName, CancellationToken? cancellationToken = null)
        {
            var lowerCaseFile = StringLocalizer[nameof(AppStrings.File)].Value.ToLowerFirstChar();

            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArtifactPathNullException(StringLocalizer.GetString(AppStrings.ArtifactPathIsNull, lowerCaseFile));

            var isExistOld = File.Exists(filePath);

            if (!isExistOld)
                throw new ArtifactDoseNotExistsException(StringLocalizer.GetString(AppStrings.ArtifactNameHasInvalidCharsException));

            if (Path.GetFileNameWithoutExtension(filePath) == newName)
                return;

            if (NameHasInvalidCharacter(newName))
                throw new ArtifactInvalidNameException(StringLocalizer.GetString(AppStrings.ArtifactNameHasInvalidCharsException));

            var artifactType = GetFsArtifactType(filePath);

            if (string.IsNullOrWhiteSpace(newName))
                throw new ArtifactNameNullException(StringLocalizer.GetString(AppStrings.ArtifactNameIsNullException));

            if (cancellationToken?.IsCancellationRequested == true) return;

            if (CheckIfNameHasInvalidChars(newName))
                throw new ArtifactInvalidNameException(StringLocalizer.GetString(AppStrings.ArtifactNameHasInvalidCharsException));

            await Task.Run(() =>
            {
                var directory = Path.GetDirectoryName(filePath);
                var isExtentionExsit = Path.HasExtension(newName);
                var newFileName = isExtentionExsit ? newName : Path.ChangeExtension(newName, Path.GetExtension(filePath));
                var newPath = Path.Combine(directory, newFileName);

                var isFileExist = File.Exists(newPath);

                if (isFileExist)
                    throw new ArtifactAlreadyExistsException(StringLocalizer.GetString(AppStrings.ArtifactAlreadyExistsException));

                LocalStorageRenameFile(filePath, newPath);
            });
        }

        public virtual async Task RenameFolderAsync(string folderPath, string newName, CancellationToken? cancellationToken = null)
        {
            var lowerCaseFolder = StringLocalizer[nameof(AppStrings.Folder)].Value.ToLowerFirstChar();

            if (string.IsNullOrWhiteSpace(folderPath))
                throw new ArtifactPathNullException(StringLocalizer.GetString(AppStrings.ArtifactPathIsNull, lowerCaseFolder));

            var isExistOld = Directory.Exists(folderPath);

            if (!isExistOld)
                throw new ArtifactDoseNotExistsException(StringLocalizer.GetString(AppStrings.ArtifactDoseNotExistsException, lowerCaseFolder));

            if (Path.GetFileNameWithoutExtension(folderPath) == newName)
                return;

            if (NameHasInvalidCharacter(newName))
                throw new ArtifactInvalidNameException(StringLocalizer.GetString(AppStrings.ArtifactNameHasInvalidCharsException));

            var artifactType = GetFsArtifactType(folderPath);

            if (artifactType is null)
                throw new ArtifactTypeNullException(StringLocalizer[nameof(AppStrings.ArtifactTypeIsNull)]);

            if (string.IsNullOrWhiteSpace(newName))
                throw new ArtifactNameNullException(StringLocalizer.GetString(AppStrings.ArtifactNameIsNullException));

            if (cancellationToken?.IsCancellationRequested == true) return;

            if (CheckIfNameHasInvalidChars(newName))
                throw new ArtifactInvalidNameException(StringLocalizer.GetString(AppStrings.ArtifactNameHasInvalidCharsException));

            var fsArtifactType = GetFsArtifactType(folderPath);

            if (fsArtifactType is FsArtifactType.Drive)
                throw new CanNotModifyOrDeleteDriveException(StringLocalizer.GetString(AppStrings.DriveRenameFailed));

            await Task.Run(() =>
            {
                var oldName = Path.GetFileName(folderPath);
                var newPath = Path.Combine(Path.GetDirectoryName(folderPath), newName);

                var isExist = Directory.Exists(newPath);

                if (isExist)
                    throw new ArtifactAlreadyExistsException(StringLocalizer.GetString(AppStrings.ArtifactAlreadyExistsException));

                LocalStorageRenameDirectory(folderPath, newPath);
            });
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
                    var fileInfo = new FileInfo(artifact.FullPath);

                    var destinationInfo = new FileInfo(Path.Combine(destination, Path.GetFileName(artifact.FullPath)));

                    var shouldCopy = true;

                    if (destinationInfo.Exists)
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
                        if (!Directory.Exists(destination))
                        {
                            LocalStorageCreateDirectory(destination);
                        }

                        try
                        {
                            await LocalStorageCopyFileAsync(fileInfo.FullName, destinationInfo.FullName);
                        }
                        catch (Exception exception)
                        {
                            ignoredList.Add((artifact, exception));
                        }
                    }
                }
                else if (artifact.ArtifactType == FsArtifactType.Folder)
                {
                    var directoryInfo = new DirectoryInfo(artifact.FullPath);
                    var destinationInfo = new DirectoryInfo(Path.Combine(destination, Path.GetFileName(artifact.FullPath)));

                    if (!Directory.Exists(destinationInfo.FullName))
                    {
                        LocalStorageCreateDirectory(destinationInfo.FullName);
                    }

                    var children = new List<FsArtifact>();

                    var directoryFiles = directoryInfo.GetFiles();

                    foreach (var file in directoryFiles)
                    {
                        if (cancellationToken?.IsCancellationRequested == true) break;

                        var providerType = GetFsFileProviderType(file.FullName);

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

                        var providerType = GetFsFileProviderType(subDirectory.FullName);

                        children.Add(new FsArtifact(subDirectory.FullName, subDirectory.Name, FsArtifactType.Folder, providerType)
                        {
                            LastModifiedDateTime = subDirectory.LastWriteTime,
                            ParentFullPath = Directory.GetParent(subDirectory.FullName)?.FullName,
                        });
                    }

                    await CopyAllAsync(children, destinationInfo.FullName, ignoredList, onShouldOverwrite, onProgress, false, cancellationToken);
                }

                if (onProgress is not null && shouldProgress)
                {
                    progressCount = await FsArtifactUtils.HandleProgressBarAsync(artifact.Name, artifacts.Count, progressCount, onProgress);
                }
            }
        }

        private async Task<List<(FsArtifact artifact, Exception exception)>?> MoveAllAsync(
            IList<FsArtifact> artifacts,
            string destination,
            Func<FsArtifact, Task<bool>>? onShouldOverwrite = null,
            List<(FsArtifact artifact, Exception exception)>? ignoredList = null,
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
                    var fileInfo = new FileInfo(artifact.FullPath);
                    var destinationInfo = new FileInfo(Path.Combine(destination, Path.GetFileName(artifact.FullPath)));

                    if (fileInfo.FullName == destinationInfo.FullName)
                        throw new SameDestinationFileException(StringLocalizer.GetString(AppStrings.SameDestinationFileException));

                    var shouldMove = true;

                    if (destinationInfo.Exists)
                    {
                        if (onShouldOverwrite is null)
                        {
                            shouldMove = false;
                            ignoredList?.Add((artifact, new ArtifactAlreadyExistsException(artifact, AppStrings.ArtifactAlreadyExistsException)));
                        }
                        else
                        {
                            shouldMove = await onShouldOverwrite(artifact);
                        }
                    }

                    if (shouldMove)
                    {
                        if (!Directory.Exists(destination))
                        {
                            LocalStorageCreateDirectory(destination);
                        }

                        try
                        {
                            await LocalStorageMoveFileAsync(fileInfo.FullName, destinationInfo.FullName);
                        }
                        catch (Exception exception)
                        {
                            ignoredList?.Add((artifact, exception));
                        }
                    }

                }
                else if (artifact.ArtifactType == FsArtifactType.Folder)
                {
                    var directoryInfo = new DirectoryInfo(artifact.FullPath);
                    var destinationInfo = new DirectoryInfo(Path.Combine(destination, Path.GetFileName(artifact.FullPath)));

                    if (directoryInfo.FullName == destinationInfo.FullName)
                        throw new SameDestinationFolderException(StringLocalizer.GetString(AppStrings.SameDestinationFolderException));

                    if (!Directory.Exists(destinationInfo.FullName))
                    {
                        LocalStorageCreateDirectory(destinationInfo.FullName);
                    }

                    var children = new List<FsArtifact>();

                    var directoryFiles = directoryInfo.GetFiles();

                    foreach (var file in directoryFiles)
                    {
                        if (cancellationToken?.IsCancellationRequested == true) break;

                        var providerType = GetFsFileProviderType(file.FullName);
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

                        var providerType = GetFsFileProviderType(subDirectory.FullName);

                        children.Add(new FsArtifact(subDirectory.FullName, subDirectory.Name, FsArtifactType.Folder, providerType)
                        {
                            LastModifiedDateTime = subDirectory.LastWriteTime,
                            ParentFullPath = Directory.GetParent(subDirectory.FullName)?.FullName,
                        });
                    }

                    var childIgnoredList = await MoveAllAsync(children,
                        destinationInfo.FullName,
                         onShouldOverwrite,
                         ignoredList,
                         onProgress,
                         false,
                         cancellationToken);

                    DeleteArtifactAsync(artifact, cancellationToken);

                }

                if (onProgress is not null && shouldProgress)
                {
                    progressCount = await FsArtifactUtils.HandleProgressBarAsync(artifact.Name, artifacts.Count, progressCount, onProgress);
                }
            }

            return ignoredList;
        }

        public virtual FsArtifactType? GetFsArtifactType(string path)
        {
            var artifactIsFile = File.Exists(path);
            if (artifactIsFile)
            {
                return FsArtifactType.File;
            }

            var drives = GetDrives();
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

        public virtual List<FsArtifact> GetDrives()
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

                var providerType = GetFsFileProviderType(drive);
                artifacts.Add(
                    new FsArtifact(drive, driveName, FsArtifactType.Drive, providerType)
                    {
                        LastModifiedDateTime = Directory.GetLastWriteTime(drive)
                    });
            }

            return artifacts;
        }

        public virtual async Task<List<FsArtifactChanges>> CheckPathExistsAsync(IEnumerable<string?> paths, CancellationToken? cancellationToken = null)
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

        protected virtual void LocalStorageDeleteFile(string path)
        {
            File.Delete(path);
        }

        protected virtual void LocalStorageDeleteDirectory(string path)
        {
            DirectoryUtils.HardDeleteDirectory(path);
        }

        protected virtual async Task LocalStorageMoveFileAsync(string filePath, string newPath)
        {
            File.Move(filePath, newPath, true);
        }

        protected virtual void LocalStorageRenameFile(string filePath, string newPath)
        {
            File.Move(filePath, newPath);
        }

        protected virtual async Task LocalStorageCopyFileAsync(string sourceFile, string destinationFile)
        {
            File.Copy(sourceFile, destinationFile, true);
        }

        protected virtual async Task LocalStorageCreateFile(string path, Stream stream)
        {
            try
            {
                using FileStream outPutFileStream = new(path, FileMode.Create);
                await stream.CopyToAsync(outPutFileStream);
            }
            catch (IOException ex)
            {
                throw new KnownIOException(ex.Message, ex);
            }
        }

        protected virtual void LocalStorageRenameDirectory(string folderPath, string newPath)
        {
            Directory.Move(folderPath, newPath);
        }

        protected virtual void LocalStorageCreateDirectory(string newPath)
        {
            Directory.CreateDirectory(newPath);
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
            var lowerCaseArtifact = StringLocalizer[nameof(AppStrings.Artifact)].Value.ToLowerFirstChar();

            if (string.IsNullOrWhiteSpace(path))
            {
                var drives = GetDrives();

                foreach (var drive in drives)
                {
                    if (cancellationToken?.IsCancellationRequested == true) yield break;
                    yield return drive;
                }
                yield break;
            }

            var fsArtifactType = GetFsArtifactType(path);

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

                    var providerType = GetFsFileProviderType(folder);

                    yield return new FsArtifact(folder, Path.GetFileName(folder), FsArtifactType.Folder, providerType)
                    {
                        ParentFullPath = Directory.GetParent(folder)?.FullName,
                        LastModifiedDateTime = Directory.GetLastWriteTime(folder),
                    };
                }

                foreach (var file in files)
                {
                    if (cancellationToken?.IsCancellationRequested == true) yield break;
                    var fileinfo = new FileInfo(file);

                    if (fileinfo.Attributes.HasFlag(FileAttributes.Hidden) ||
                        fileinfo.Attributes.HasFlag(FileAttributes.System) ||
                        fileinfo.Attributes.HasFlag(FileAttributes.Temporary)) continue;

                    var providerType = GetFsFileProviderType(file);

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
                var providerType = GetFsFileProviderType(path);
                yield return new FsArtifact(path, Path.GetFileName(path), FsArtifactType.File, providerType)
                {
                    ParentFullPath = Directory.GetParent(path)?.FullName,
                    LastModifiedDateTime = File.GetLastWriteTime(path),
                    FileExtension = Path.GetExtension(path),
                    Size = fileinfo.Length
                };
            }
        }

        private async IAsyncEnumerable<FsArtifact> GetAllFileAndFoldersAsync(string path, DeepSearchFilter? deepSearchFilter, CancellationToken? cancellationToken = null)
        {
            if (deepSearchFilter is null)
                throw new InvalidOperationException("The search filter is empty.");

            if (cancellationToken?.IsCancellationRequested == true) yield break;

            var inLineDeepSearchFilter = new DeepSearchFilter
            {
                SearchText = deepSearchFilter.SearchText,
                ArtifactCategorySearchTypes = deepSearchFilter.ArtifactCategorySearchTypes,
                ArtifactDateSearchType = deepSearchFilter.ArtifactDateSearchType
            };

            var allFileAndFolders = Directory.EnumerateFileSystemEntries(path,
                !string.IsNullOrWhiteSpace(inLineDeepSearchFilter.SearchText) ? $"*{inLineDeepSearchFilter.SearchText}*" : "*",
                new EnumerationOptions
                {
                    IgnoreInaccessible = true,
                    AttributesToSkip = FileAttributes.Hidden | FileAttributes.System | FileAttributes.Temporary,
                    MatchCasing = MatchCasing.CaseInsensitive,
                    RecurseSubdirectories = true
                })
                .Select(fullPath => new
                {
                    FullPath = fullPath,
                    ArtifactInfo = new DirectoryInfo(fullPath)
                });

            if (inLineDeepSearchFilter.ArtifactCategorySearchTypes is not null && inLineDeepSearchFilter.ArtifactCategorySearchTypes.Any())
            {
                if (cancellationToken?.IsCancellationRequested == true) yield break;

                List<string> categoryTypeList = new();

                foreach (var type in inLineDeepSearchFilter.ArtifactCategorySearchTypes)
                {
                    var types = FsArtifactUtils.GetSearchCategoryTypeExtensions(type);

                    types.ForEach(categoryTypeList.Add);
                }

                allFileAndFolders = allFileAndFolders.Where(f => categoryTypeList.Any() &&
                                                            categoryTypeList.Contains(f.ArtifactInfo.Extension.ToLower()));
            }

            if (inLineDeepSearchFilter.ArtifactDateSearchType.HasValue)
            {
                if (cancellationToken?.IsCancellationRequested == true) yield break;

                var dateDiff = inLineDeepSearchFilter.ArtifactDateSearchType switch
                {
                    ArtifactDateSearchType.Yesterday => 1,
                    ArtifactDateSearchType.Past7Days => 7,
                    ArtifactDateSearchType.Past30Days => 30,
                    _ => 0
                };

                allFileAndFolders = allFileAndFolders.Where(f => f.ArtifactInfo.LastWriteTime >= DateTimeOffset.Now.AddDays(-dateDiff));
            }

            foreach (var artifact in allFileAndFolders)
            {
                if (cancellationToken?.IsCancellationRequested == true) yield break;

                if (artifact.ArtifactInfo is null) continue;

                if (artifact?.ArtifactInfo?.Attributes == (FileAttributes)(-1)) continue;

                var providerType = GetFsFileProviderType(artifact.FullPath);
                var artifactType = GetFsArtifactType(artifact.FullPath);

                yield return new FsArtifact(artifact.FullPath, artifact.ArtifactInfo.Name, artifactType.Value, providerType)
                {
                    ParentFullPath = artifact.ArtifactInfo?.Parent?.FullName,
                    LastModifiedDateTime = artifact.ArtifactInfo?.LastWriteTime ?? default,
                    FileExtension = artifactType == FsArtifactType.File ? artifact.ArtifactInfo?.Extension : null,
                    Size = artifactType == FsArtifactType.File ? new FileInfo(artifact.FullPath).Length : null
                };
            }
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

        public virtual async IAsyncEnumerable<FsArtifact> GetSearchArtifactAsync(DeepSearchFilter? deepSearchFilter, CancellationToken? cancellationToken = null)
        {
            var drives = GetDrives();

            foreach (var drive in drives)
            {
                if (cancellationToken?.IsCancellationRequested == true) yield break;

                await foreach (var artifact in GetAllFileAndFoldersAsync(drive.FullPath, deepSearchFilter, cancellationToken))
                {
                    if (cancellationToken?.IsCancellationRequested == true) yield break;

                    yield return artifact;
                }
            }
            yield break;
        }

        public Task<long> GetArtifactSizeAsync(string path, Action<long>? onProgress = null, CancellationToken? cancellationToken = null)
        {
            if (path is null)
                throw new ArtifactPathNullException("Artifact path is null.");

            if (cancellationToken?.IsCancellationRequested is true)
                return Task.FromResult<long>(0);

            long artifactSize = 0;
            var artifactType = GetFsArtifactType(path);

            if (artifactType == FsArtifactType.Folder)
            {
                var allFiles = Directory.EnumerateFileSystemEntries(path, "*", new EnumerationOptions()
                {
                    RecurseSubdirectories = true
                }).Select(a => new FileInfo(a));

                foreach (var item in allFiles)
                {
                    if (cancellationToken?.IsCancellationRequested is true)
                        break;

                    if (!File.Exists(item.FullName))
                        continue;

                    artifactSize += item.Length;

                    onProgress?.Invoke(artifactSize);
                }
            }
            else if (artifactType == FsArtifactType.Drive)
            {
                artifactSize = CalculateDriveSize(path, cancellationToken);

                onProgress?.Invoke(artifactSize);
            }
            else if (artifactType == FsArtifactType.File)
            {
                var file = new FileInfo(path);
                artifactSize = file.Length;

                onProgress?.Invoke(artifactSize);
            }
            else
            {
                throw new InvalidOperationException($"Unknown artifact type to calculate size: {artifactType}");
            }

            return Task.FromResult(artifactSize);
        }

        protected virtual long CalculateDriveSize(string drivePath, CancellationToken? cancellation = null)
        {
            long totalSize = 0;
            var drives = DriveInfo.GetDrives();
            var targetDrive = drives.FirstOrDefault(drive => drive.Name.Equals(drivePath, StringComparison.OrdinalIgnoreCase));

            if (targetDrive is null)
                throw new InvalidOperationException("No drive found given the current path.");

            totalSize = targetDrive.TotalSize - targetDrive.TotalFreeSpace;
            return totalSize;
        }

        private static bool NameHasInvalidCharacter(string fileName)
        {
            if (fileName.Contains('>') ||
               fileName.Contains('<') ||
               fileName.Contains(':') ||
               fileName.Contains('?') ||
               fileName.Contains('"') ||
               fileName.Contains('*'))
            {
                return true;
            }
            return false;
        }

        public virtual string GetShowablePath(string artifactPath)
        {
            return artifactPath;
        }
    }
}
