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

            FsArtifact newFsArtifact = new();
            var fileName = Path.GetFileNameWithoutExtension(path);

            if (string.IsNullOrWhiteSpace(fileName))
                throw new DomainLogicException(StringLocalizer.GetString(AppStrings.ArtifactNameIsNull, "file"));

            try
            {
                if (File.Exists(path))
                    throw new DomainLogicException(StringLocalizer.GetString(AppStrings.ArtifactAlreadyExistsException, "file"));

                using FileStream outPutFileStream = new(path, FileMode.Create);
                await stream.CopyToAsync(outPutFileStream);

                newFsArtifact = new FsArtifact
                {
                    Name = fileName,
                    FullPath = path,
                    ArtifactType = FsArtifactType.File,
                    FileExtension = Path.GetExtension(path),
                    Size = (int)outPutFileStream.Length,
                    ProviderType = await GetFsFileProviderTypeAsync(path),
                    LastModifiedDateTime = File.GetLastWriteTime(path),
                    ParentFullPath = Directory.GetParent(path)?.FullName,
                };
            }
            catch
            {
                // ToDo : Handle exceptions
                throw;
            }

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

            FsArtifact newFsArtifact = new();

            if (string.IsNullOrWhiteSpace(folderName))
                throw new DomainLogicException(StringLocalizer.GetString(AppStrings.ArtifactNameIsNull, "folder"));

            var newPath = Path.Combine(path, folderName);

            try
            {
                if (Directory.Exists(newPath))
                    throw new DomainLogicException(StringLocalizer.GetString(AppStrings.ArtifactAlreadyExistsException, "folder"));

                Directory.CreateDirectory(newPath);
                newFsArtifact = new FsArtifact()
                {
                    Name = folderName,
                    FullPath = newPath,
                    ArtifactType = FsArtifactType.Folder,
                    ProviderType = await GetFsFileProviderTypeAsync(newPath),
                    ParentFullPath = Directory.GetParent(newPath)?.FullName,
                    LastModifiedDateTime = Directory.GetLastWriteTime(newPath)
                };
            }
            catch (Exception)
            {
                throw;
                // ToDo : Handle exception
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
                throw new DomainLogicException(StringLocalizer.GetString(AppStrings.ArtifactPathIsNull, artifact?.ArtifactType?.ToString() ?? ""));

            if (artifact.ArtifactType == null)
                throw new DomainLogicException(StringLocalizer[nameof(AppStrings.ArtifactTypeIsNull)]);

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

        public virtual async Task<Stream> GetFileContentAsync(string filePath, CancellationToken? cancellationToken = null)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new DomainLogicException(StringLocalizer.GetString(AppStrings.ArtifactPathIsNull, "file"));

            var streamReader = new StreamReader(filePath);
            return streamReader.BaseStream;
        }

        public virtual async Task MoveArtifactsAsync(FsArtifact[] artifacts, string destination, bool beOverWritten = false, CancellationToken? cancellationToken = null)
        {
            foreach (var artifact in artifacts)
            {
                if (artifact.FullPath == null) continue;

                CopyAll(new DirectoryInfo(artifact.FullPath), new DirectoryInfo(destination));
                DeleteArtifactAsync(artifact, cancellationToken);
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

                var oldName = Path.GetFileNameWithoutExtension(filePath);
                var newPath = filePath.Replace(oldName, newName);

                File.Move(filePath, newPath);
        }

        public virtual async Task RenameFolderAsync(string folderPath, string newName, CancellationToken? cancellationToken = null)
        {
            if (string.IsNullOrWhiteSpace(folderPath))
                throw new DomainLogicException(StringLocalizer.GetString(AppStrings.ArtifactPathIsNull, ""));

            var artifactType = GetFsArtifactTypeAsync(folderPath);

            if (string.IsNullOrWhiteSpace(newName))
                throw new DomainLogicException(StringLocalizer.GetString(AppStrings.ArtifactNameIsNull, artifactType.ToString() ?? ""));

            if (cancellationToken?.IsCancellationRequested == true) return;

                var oldName = Path.GetFileName(folderPath);
                var newPath = folderPath.Replace(oldName, newName);

                Directory.Move(folderPath, newPath);
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

        public virtual async Task<FsArtifactType> GetFsArtifactTypeAsync(string path)
        {
            string[] drives = Directory.GetLogicalDrives();

            if (drives.Contains(path))
            {
                return FsArtifactType.Drive;
            }

            FileAttributes attr = File.GetAttributes(path);

            if (attr.HasFlag(FileAttributes.Directory))
            {
                return FsArtifactType.Folder;
            }
            else
            {
                return FsArtifactType.File;
            }
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
    }
}
