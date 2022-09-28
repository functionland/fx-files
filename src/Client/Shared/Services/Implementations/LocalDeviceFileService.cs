using System.IO;
using System.Text;

namespace Functionland.FxFiles.Client.Shared.Services.Implementations
{
    public abstract partial class LocalDeviceFileService : IFileService
    {
        [AutoInject] public IStringLocalizer<AppStrings> StringLocalizer { get; set; } = default!;

        public abstract Task<FsFileProviderType> GetFsFileProviderTypeAsync(string filePath);

        public virtual async Task CopyArtifactsAsync(FsArtifact[] artifacts, string destination, bool overwrite = false, CancellationToken? cancellationToken = null)
        {
            List<FsArtifact> ignoredList = new();

            await Task.Run(async () =>
            {
                ignoredList = await CopyAllAsync(artifacts, destination, overwrite, cancellationToken);
            });

            if (ignoredList.Any())
            {
                throw new CanNotOperateOnFilesException(StringLocalizer[nameof(AppStrings.CanNotOperateOnFilesException)], ignoredList);
            }
        }

        public virtual async Task<FsArtifact> CreateFileAsync(string path, Stream stream, CancellationToken? cancellationToken = null)
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

            if (File.Exists(path))
                throw new DomainLogicException(StringLocalizer.GetString(AppStrings.ArtifactAlreadyExistsException, "file"));

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
            if (string.IsNullOrWhiteSpace(path))
                throw new DomainLogicException(StringLocalizer.GetString(AppStrings.ArtifactPathIsNull, "folder"));


            if (string.IsNullOrWhiteSpace(folderName))
                throw new DomainLogicException(StringLocalizer.GetString(AppStrings.ArtifactNameIsNull, "folder"));

            if (CheckIfNameHasInvalidChars(folderName))
                throw new DomainLogicException(StringLocalizer.GetString(AppStrings.ArtifactNameHasInvalidChars, "folder"));

            var newPath = Path.Combine(path, folderName);

            if (Directory.Exists(newPath))
                throw new DomainLogicException(StringLocalizer.GetString(AppStrings.ArtifactAlreadyExistsException, "folder"));

            Directory.CreateDirectory(newPath);

            var newFsArtifact = new FsArtifact(newPath, folderName, FsArtifactType.Folder, await GetFsFileProviderTypeAsync(newPath))
            {
                ParentFullPath = Directory.GetParent(newPath)?.FullName,
                LastModifiedDateTime = Directory.GetLastWriteTime(newPath)
            };

            return newFsArtifact;
        }

        public virtual async Task DeleteArtifactsAsync(FsArtifact[] artifacts, CancellationToken? cancellationToken = null)
        {
            foreach (var artifact in artifacts)
            {
                if (cancellationToken?.IsCancellationRequested == true)
                    break;

                DeleteArtifactAsync(artifact);
            }
        }

        private void DeleteArtifactAsync(FsArtifact artifact, CancellationToken? cancellationToken = null)
        {
            if (string.IsNullOrWhiteSpace(artifact.FullPath))
                throw new DomainLogicException(StringLocalizer.GetString(AppStrings.ArtifactPathIsNull, artifact?.ArtifactType.ToString() ?? ""));

            var isDirectoryExist = Directory.Exists(artifact.FullPath);
            var isFileExist = File.Exists(artifact.FullPath);

            if ((artifact.ArtifactType == FsArtifactType.Folder && !isDirectoryExist) ||
                (artifact.ArtifactType == FsArtifactType.File && !isFileExist))
                throw new DomainLogicException(StringLocalizer.GetString(AppStrings.ArtifactDoseNotExistsException, artifact?.ArtifactType.ToString() ?? "artifact"));

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
                throw new DomainLogicException(StringLocalizer[nameof(AppStrings.DriveRemoveFailed)]);
            }
        }

        public virtual async IAsyncEnumerable<FsArtifact> GetArtifactsAsync(string? path = null, string? searchText = null, CancellationToken? cancellationToken = null)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                var drives = await GetDrivesAsync();

                foreach (var drive in drives)
                {
                    drive.LastModifiedDateTime = Directory.GetLastWriteTime(drive.FullPath);
                    yield return drive;
                }
                yield break;
            }

            var artifacts = new List<FsArtifact>();
            var subArtifacts = new List<FsArtifact>();

            var fsArtifactType = await GetFsArtifactTypeAsync(path);

            if (fsArtifactType is null)
                throw new DomainLogicException(StringLocalizer.GetString(AppStrings.ArtifactDoseNotExistsException, fsArtifactType?.ToString() ?? "artifact"));

            if (fsArtifactType is FsArtifactType.Folder or FsArtifactType.Drive)
            {
                string[] directoryFiles = Directory.GetFiles(path);
                string[] subDirectories = Directory.GetDirectories(path);

                foreach (var subDirectory in subDirectories)
                {
                    var directoryInfo = new DirectoryInfo(subDirectory);

                    if (directoryInfo.Attributes.HasFlag(FileAttributes.Hidden)) continue;

                    var providerType = await GetFsFileProviderTypeAsync(subDirectory);

                    subArtifacts.Add(
                        new FsArtifact(subDirectory, Path.GetFileName(subDirectory), FsArtifactType.Folder, providerType)
                        {
                            ParentFullPath = Directory.GetParent(subDirectory)?.FullName,
                            LastModifiedDateTime = Directory.GetLastWriteTime(subDirectory)
                        });
                }

                subArtifacts = subArtifacts.OrderBy(i => i.Name).ToList();

                foreach (var file in directoryFiles)
                {
                    var fileinfo = new FileInfo(file);

                    if (fileinfo.Attributes.HasFlag(FileAttributes.Hidden)) continue;

                    var providerType = await GetFsFileProviderTypeAsync(file);

                    artifacts.Add(
                        new FsArtifact(file, Path.GetFileName(file), FsArtifactType.File, providerType)
                        {
                            ParentFullPath = Directory.GetParent(file)?.FullName,
                            LastModifiedDateTime = File.GetLastWriteTime(file),
                            FileExtension = Path.GetExtension(file),
                            Size = fileinfo.Length
                        });
                }

                artifacts = artifacts.OrderBy(i => i.Name).ToList();

                var result = subArtifacts.Concat(artifacts);

                if (!string.IsNullOrWhiteSpace(searchText))
                {
                    result = result.Where(i => i.Name.ToLower().Contains(searchText.ToLower())).ToList();
                }

                foreach (var item in result)
                {
                    yield return item;
                }
            }
            else
            {
                var fileInfo = new FileInfo(path);

                yield return new FsArtifact(path, Path.GetFileName(path), FsArtifactType.File, await GetFsFileProviderTypeAsync(path))
                {
                    ParentFullPath = Directory.GetParent(path)?.FullName,
                    LastModifiedDateTime = File.GetLastWriteTime(path),
                    FileExtension = Path.GetExtension(path),
                    Size = fileInfo.Length
                };
            }
        }

        public virtual async Task<FsArtifact> GetFsArtifactAsync(string? path, CancellationToken? cancellationToken = null)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new DomainLogicException(StringLocalizer.GetString(AppStrings.ArtifactPathIsNull, ""));

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
                fsArtifact.Name = await GetDriveNameAsync(path);
            }

            return fsArtifact;
        }

        public virtual async Task<Stream> GetFileContentAsync(string filePath, CancellationToken? cancellationToken = null)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new DomainLogicException(StringLocalizer.GetString(AppStrings.ArtifactPathIsNull, "file"));

            var streamReader = new StreamReader(filePath);
            return streamReader.BaseStream;
        }

        public virtual async Task MoveArtifactsAsync(FsArtifact[] artifacts, string destination, bool overwrite = false, CancellationToken? cancellationToken = null)
        {
            List<FsArtifact> ignoredList = new();

            await Task.Run(async () =>
            {
                ignoredList = await CopyAllAsync(artifacts, destination, overwrite, cancellationToken);
                await DeleteArtifactsAsync(artifacts, cancellationToken);
            });

            if (ignoredList.Any())
            {
                throw new CanNotOperateOnFilesException(StringLocalizer[nameof(AppStrings.CanNotOperateOnFilesException)], ignoredList);
            }
        }

        public virtual async Task RenameFileAsync(string filePath, string newName, CancellationToken? cancellationToken = null)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new DomainLogicException(StringLocalizer.GetString(AppStrings.ArtifactPathIsNull, "file"));

            var artifactType = GetFsArtifactTypeAsync(filePath);

            if (string.IsNullOrWhiteSpace(newName))
                throw new DomainLogicException(StringLocalizer.GetString(AppStrings.ArtifactNameIsNull, "file"));

            if (cancellationToken?.IsCancellationRequested == true) return;

            if (CheckIfNameHasInvalidChars(newName))
                throw new DomainLogicException(StringLocalizer.GetString(AppStrings.ArtifactNameHasInvalidChars, "file"));

            var isExistOld = File.Exists(filePath);

            if (!isExistOld)
                throw new DomainLogicException(StringLocalizer.GetString(AppStrings.ArtifactDoseNotExistsException, "file"));

            await Task.Run(() =>
            {

                var directory = Path.GetDirectoryName(filePath);
                var isExtentionExsit = Path.HasExtension(newName);
                var newFileName = isExtentionExsit ? newName : Path.ChangeExtension(newName, Path.GetExtension(filePath));
                var newPath = Path.Combine(directory, newFileName);

                var isFileExist = File.Exists(newPath);

                if (isFileExist)
                    throw new DomainLogicException(StringLocalizer.GetString(AppStrings.ArtifactAlreadyExistsException, "file"));

                File.Move(filePath, newPath);
            });
        }

        public virtual async Task RenameFolderAsync(string folderPath, string newName, CancellationToken? cancellationToken = null)
        {
            if (string.IsNullOrWhiteSpace(folderPath))
                throw new DomainLogicException(StringLocalizer.GetString(AppStrings.ArtifactPathIsNull, "folder"));

            var artifactType = GetFsArtifactTypeAsync(folderPath);

            if (artifactType is null)
                throw new DomainLogicException(StringLocalizer[nameof(AppStrings.ArtifactTypeIsNull)]);

            if (string.IsNullOrWhiteSpace(newName))
                throw new DomainLogicException(StringLocalizer.GetString(AppStrings.ArtifactNameIsNull, "folder"));

            if (cancellationToken?.IsCancellationRequested == true) return;

            if (CheckIfNameHasInvalidChars(newName))
                throw new DomainLogicException(StringLocalizer.GetString(AppStrings.ArtifactNameHasInvalidChars, "folder"));

            var isExistOld = Directory.Exists(folderPath);

            if (!isExistOld)
                throw new DomainLogicException(StringLocalizer.GetString(AppStrings.ArtifactDoseNotExistsException, "folder"));

            var fsArtifactType = await GetFsArtifactTypeAsync(folderPath);

            if (fsArtifactType is FsArtifactType.Drive)
                throw new DomainLogicException(StringLocalizer.GetString(AppStrings.DriveRenameFailed));

            await Task.Run(() =>
            {
                var oldName = Path.GetFileName(folderPath);
                var newPath = Path.Combine(Path.GetDirectoryName(folderPath), newName);

                var isExist = Directory.Exists(newPath);

                if (isExist)
                    throw new DomainLogicException(StringLocalizer.GetString(AppStrings.ArtifactAlreadyExistsException, "folder"));

                Directory.Move(folderPath, newPath);
            });
        }

        private async Task<List<FsArtifact>> CopyAllAsync(IEnumerable<FsArtifact> artifacts, string destination, bool overwrite = false, CancellationToken? cancellationToken = null)
        {
            var ignoredList = new List<FsArtifact>();

            foreach (var artifact in artifacts)
            {
                if (cancellationToken?.IsCancellationRequested == true) break;
                
                if (artifact.ArtifactType == FsArtifactType.File)
                {
                    var fileInfo = new FileInfo(artifact.FullPath);
                    var destinationInfo = new FileInfo(Path.Combine(destination, Path.GetFileName(artifact.FullPath)));

                    if (fileInfo.FullName == destinationInfo.FullName)
                        throw new DomainLogicException(StringLocalizer.GetString(AppStrings.SameDestinationFileException));

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
                    }
                }
                else if (artifact.ArtifactType == FsArtifactType.Folder)
                {
                    var directoryInfo = new DirectoryInfo(artifact.FullPath);
                    var destinationInfo = new DirectoryInfo(Path.Combine(destination, Path.GetFileName(artifact.FullPath)));

                    if (directoryInfo.FullName == destinationInfo.FullName)
                        throw new DomainLogicException(StringLocalizer.GetString(AppStrings.SameDestinationFolderException));

                    if (!destinationInfo.Exists)
                    {
                        destinationInfo.Create();
                    }

                    var children = new List<FsArtifact>();

                    var directoryFiles = directoryInfo.GetFiles();

                    foreach (var file in directoryFiles)
                    {
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
                        var providerType = await GetFsFileProviderTypeAsync(subDirectory.FullName);

                        children.Add(new FsArtifact(subDirectory.FullName, subDirectory.Name, FsArtifactType.Folder, providerType)
                        {
                            LastModifiedDateTime = subDirectory.LastWriteTime,
                            ParentFullPath = Directory.GetParent(subDirectory.FullName)?.FullName,
                        });
                    }

                    var childIgnoredList = await CopyAllAsync(children, destinationInfo.FullName, overwrite, cancellationToken);
                    ignoredList.AddRange(childIgnoredList);
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

            string[] drives = Directory.GetLogicalDrives();
            if (drives.Contains(path))
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
                var driveName = !string.IsNullOrWhiteSpace(lable) ? lable : drive;

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
                    throw new DomainLogicException(StringLocalizer.GetString(AppStrings.ArtifactPathIsNull, ""));

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

        private bool CheckIfNameHasInvalidChars(string name)
        {
            var invalidChars = Path.GetInvalidFileNameChars();

            foreach (var invalid in invalidChars)
                if (name.Contains(invalid)) return true;
            return false;
        }

        private async Task<string> GetDriveNameAsync(string? path)
        {
            var drives = await GetDrivesAsync();
            var drive = drives.Where(d => d.FullPath == path).FirstOrDefault();

            var driveInfo = new DriveInfo(drive.FullPath);
            var driveFullPath = drive.FullPath.TrimEnd(Path.DirectorySeparatorChar);
            var driveName = !string.IsNullOrWhiteSpace(driveInfo.VolumeLabel) ? $"{driveInfo.VolumeLabel} ({driveFullPath})" : driveFullPath;
            return driveName;
        }
    }
}
