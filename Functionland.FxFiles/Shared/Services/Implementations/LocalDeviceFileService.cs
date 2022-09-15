using Functionland.FxFiles.Shared.Models;
using Microsoft.AspNetCore.StaticFiles;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Functionland.FxFiles.Shared.Services.Implementations
{
    public abstract partial class LocalDeviceFileService : IFileService
    {
        [AutoInject] public IStringLocalizer<AppStrings> StringLocalizer { get; set; } = default!;

        public abstract Task<FsFileProviderType> GetFsFileProviderTypeAsync(string filePath);

        public virtual async Task CopyArtifactsAsync(FsArtifact[] artifacts, string destination, bool overwrite = false, CancellationToken? cancellationToken = null)
        {
            List<FsArtifact> ignoredList = new();

            await Task.Run(() =>
            {
                ignoredList = CopyAll(artifacts, destination, overwrite, cancellationToken);
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

                if (File.Exists(path))
                    throw new DomainLogicException(StringLocalizer.GetString(AppStrings.ArtifactAlreadyExistsException, "file"));

                using FileStream outPutFileStream = new(path, FileMode.Create);
                await stream.CopyToAsync(outPutFileStream);

            var newFsArtifact = new FsArtifact(path, fileName, FsArtifactType.File, await GetFsFileProviderTypeAsync(path))
                {
                    FileExtension = Path.GetExtension(path),
                    Size = (int)outPutFileStream.Length,
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
                    yield return drive;
                yield break;
            }

            var artifacts = new List<FsArtifact>();
            var subArtifacts = new List<FsArtifact>();

            var fsArtifactType = await GetFsArtifactTypeAsync(path);

            if (fsArtifactType is null)
                throw new DomainLogicException(StringLocalizer[nameof(AppStrings.ArtifactTypeIsNull)]);

            if (fsArtifactType is FsArtifactType.Folder or FsArtifactType.Drive)
            {
                string[] directoryFiles = Directory.GetFiles(path);
                string[] subDirectories = Directory.GetDirectories(path);

                foreach (var subDirectory in subDirectories)
                {
                    subArtifacts.Add(
                        new FsArtifact()
                        {
                            ArtifactType = FsArtifactType.Folder,
                            FullPath = subDirectory,
                            ProviderType = await GetFsFileProviderTypeAsync(subDirectory),
                            Name = Path.GetFileName(subDirectory),
                            ParentFullPath = Directory.GetParent(subDirectory)?.FullName,
                            LastModifiedDateTime = Directory.GetLastWriteTime(subDirectory)
                        });
                }

                subArtifacts = subArtifacts.OrderBy(i => i.Name).ToList();

                foreach (var file in directoryFiles)
                {
                    artifacts.Add(
                        new FsArtifact()
                        {
                            ArtifactType = FsArtifactType.File,
                            FullPath = file,
                            ProviderType = await GetFsFileProviderTypeAsync(file),
                            Name = Path.GetFileName(file),
                            ParentFullPath = Directory.GetParent(file)?.FullName,
                            LastModifiedDateTime = File.GetLastWriteTime(file),
                            FileExtension = Path.GetExtension(file)
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
                yield return new FsArtifact()
                {
                    ArtifactType = FsArtifactType.File,
                    FullPath = path,
                    ProviderType = await GetFsFileProviderTypeAsync(path),
                    Name = Path.GetFileName(path),
                    ParentFullPath = Directory.GetParent(path)?.FullName,
                    LastModifiedDateTime = File.GetLastWriteTime(path),
                    FileExtension = Path.GetExtension(path)
                };
            }
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
                ignoredList = CopyAll(artifacts, destination, overwrite, cancellationToken);
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
                throw new DomainLogicException(StringLocalizer.GetString(AppStrings.ArtifactPathIsNull, ""));

            var artifactType = GetFsArtifactTypeAsync(filePath);

            if (string.IsNullOrWhiteSpace(newName))
                throw new DomainLogicException(StringLocalizer.GetString(AppStrings.ArtifactNameIsNull, artifactType.ToString() ?? ""));

            if (cancellationToken?.IsCancellationRequested == true) return;

            await Task.Run(() =>
            {
                var oldName = Path.GetFileNameWithoutExtension(filePath);
                var newPath = filePath.Replace(oldName, newName);

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

            await Task.Run(() =>
            {
                var oldName = Path.GetFileName(folderPath);
                var newPath = folderPath.Replace(oldName, newName);

                var isExist = Directory.Exists(newPath);

                if (isExist)
                    throw new DomainLogicException(StringLocalizer.GetString(AppStrings.ArtifactAlreadyExistsException, "folder"));

                Directory.Move(folderPath, newPath);
            });
        }

        private static List<FsArtifact> CopyAll(IEnumerable<FsArtifact> artifacts, string destination, bool overwrite = false, CancellationToken? cancellationToken = null)
        {
            var ignoredList = new List<FsArtifact>();

            foreach (var artifact in artifacts)
            {
                if (cancellationToken?.IsCancellationRequested == true) break;

                if (artifact.ArtifactType == FsArtifactType.File)
                {
                    var fileInfo = new FileInfo(artifact.FullPath);
                    var destinationInfo = new FileInfo(Path.Combine(destination, Path.GetFileName(artifact.FullPath)));

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

                    if (!destinationInfo.Exists)
                    {
                        destinationInfo.Create();
                    }

                    var children = new List<FsArtifact>();
                    children.AddRange(
                        from file in directoryInfo.GetFiles()
                        select new FsArtifact()
                        {
                            FullPath = file.FullName,
                            ArtifactType = FsArtifactType.File,
                            Name = file.Name,
                            FileExtension = file.Extension,
                            LastModifiedDateTime = file.LastWriteTime,
                            ParentFullPath = Directory.GetParent(file.FullName)?.FullName
                        });

                    children.AddRange(
                        from subDirectory in directoryInfo.GetDirectories()
                        select new FsArtifact()
                        {
                            FullPath = subDirectory.FullName,
                            ArtifactType = FsArtifactType.Folder,
                            Name = subDirectory.Name,
                            LastModifiedDateTime = subDirectory.LastWriteTime,
                            ParentFullPath = Directory.GetParent(subDirectory.FullName)?.FullName
                        });

                    var childIgnoredList = CopyAll(children, destinationInfo.FullName, overwrite, cancellationToken);
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

            var artifactIsDirectory = Directory.Exists(path);
            if (artifactIsDirectory)
            {
                return FsArtifactType.Folder;
            }

            string[] drives = Directory.GetLogicalDrives();
            if (drives.Contains(path))
            {
                return FsArtifactType.Drive;
            }

            return null;
        }

        public virtual async Task<List<FsArtifact>> GetDrivesAsync()
        {
            var drives = Directory.GetLogicalDrives();
            var artifacts = new List<FsArtifact>();

            foreach (var drive in drives)
            {
                artifacts.Add(new FsArtifact()
                {
                    ArtifactType = FsArtifactType.Drive,
                    FullPath = drive,
                    ProviderType = await GetFsFileProviderTypeAsync(drive)
                });
            }

            return artifacts;
        }

        public virtual async Task<FsArtifactChanges> CheckPathExistsAsync(string? path, CancellationToken? cancellationToken = null)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new DomainLogicException(StringLocalizer.GetString(AppStrings.ArtifactPathIsNull, ""));

            var fileInfo = new FileInfo(path);
            var directoryInfo = new DirectoryInfo(path);

            var artifactIsFile = File.Exists(path);
            var artifactIsDirectory = Directory.Exists(path);

            var fsArtifact = new FsArtifactChanges()
            {
                ArtifactFullPath = path,
            };

            if (artifactIsFile && fileInfo.Exists)
            {
                fsArtifact.IsPathExist = true;
            }
            else if (artifactIsDirectory && directoryInfo.Exists)
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

            return fsArtifact;
        }

        private bool CheckIfNameHasInvalidChars(string name)
        {
            var invalidChars = Path.GetInvalidFileNameChars();

            foreach (var invalid in invalidChars)
                if (name.Contains(invalid)) return true;
            return false;
        }
    }
}
