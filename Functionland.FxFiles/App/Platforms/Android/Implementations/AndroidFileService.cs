using Android.Content;
using Android.OS;
using Android.Provider;
using android = Android;

using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using AndroidX.DocumentFile.Provider;
using Android.OS.Storage;
using Android.Media;
using Stream = System.IO.Stream;
using Java.Interop;

namespace Functionland.FxFiles.App.Platforms.Android.Implementations
{
    public partial class AndroidFileService : LocalDeviceFileService
    {
        private async Task<List<FsArtifact>> GetDrivesAsync()
        {
            var storageManager = MauiApplication.Current.GetSystemService(Context.StorageService) as StorageManager;
            if (storageManager is null)
            {
                throw new DomainLogicException(StringLocalizer[nameof(AppStrings.UnableToLoadStorageManager)]);
            }

            var storageVolumes = storageManager.StorageVolumes.ToList();

            var drives = new List<FsArtifact>();
            foreach (var storage in storageVolumes)
            {
                if (storage.IsPrimary) //TODO: investigate to use IsPrimary or IsRemoveable
                {
                    var time = storage.Directory.LastModified();
                    //ToDo: Fill FsArtifact extral fields 
                    drives.Add(new FsArtifact()
                    {
                        Name = "internal",
                        ArtifactType = FsArtifactType.Drive,
                        ProviderType = FsFileProviderType.InternalMemory,
                        FullPath = storage.Directory.Path,
                        Capacity = storage.Directory.UsableSpace, //TODO : investigate to use UsableSpace or freeSpace
                        Size = storage.Directory.TotalSpace,
                        //LastModifiedDateTime = storage.Directory.LastModified() //TODO: convert long to dateTime
                    });
                }
                else
                {
                    drives.Add(new FsArtifact()
                    {
                        Name = $"SD Card ({storage.MediaStoreVolumeName})",
                        ArtifactType = FsArtifactType.Drive,
                        ProviderType = FsFileProviderType.ExternalMemory,
                        FullPath = storage.Directory.Path,
                        Capacity = storage.Directory.UsableSpace,//TODO : investigate to use UsableSpace or freeSpace
                        Size = storage.Directory.TotalSpace,
                        //LastModifiedDateTime = storage.Directory.LastModified() //TODO: convert long to dateTime
                    });
                }

                //ToDo: Hanlde when drive is Blox
            }

            return drives;
        }

        private async Task<FsArtifactType> GetFsArtifactTypeAsync(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new DomainLogicException(StringLocalizer[nameof(AppStrings.PathIsNull)]);
            }
            else if (Directory.Exists(path))
            {
                return FsArtifactType.Folder;
            }
            else if (File.Exists(path))
            {
                return FsArtifactType.File;
            }
            else
            {
                return FsArtifactType.Drive;
            }
        }

        public override async Task CopyArtifactsAsync(FsArtifact[] artifacts, string destination, CancellationToken? cancellationToken = null)
        {
            await base.CopyArtifactsAsync(artifacts, destination, cancellationToken);
        }

        public override async Task<FsArtifact> CreateFileAsync(string path, Stream stream, CancellationToken? cancellationToken = null)
        {
            return await base.CreateFileAsync(path, stream, cancellationToken);
        }

        public override Task<List<FsArtifact>> CreateFilesAsync(IEnumerable<(string path, Stream stream)> files, CancellationToken? cancellationToken = null)
        {
            return base.CreateFilesAsync(files, cancellationToken);
        }

        public override Task DeleteArtifactsAsync(FsArtifact[] artifacts, CancellationToken? cancellationToken = null)
        {
            return base.DeleteArtifactsAsync(artifacts, cancellationToken);
        }

        public override async IAsyncEnumerable<FsArtifact> GetArtifactsAsync(string? path = null, string? searchText = null, CancellationToken? cancellationToken = null)
        {
            if (path is null)
            {
                var drives = await GetDrivesAsync();
                foreach (var drive in drives)
                {
                    yield return drive;
                }
                yield break;
            }

            await foreach (var artifact in base.GetArtifactsAsync(path, searchText, cancellationToken))
            {
                yield return artifact;
            }
        }

        public override Task MoveArtifactsAsync(FsArtifact[] artifacts, string destination, CancellationToken? cancellationToken = null)
        {
            return base.MoveArtifactsAsync(artifacts, destination, cancellationToken);
        }

        public override Task RenameFileAsync(string filePath, string newName, CancellationToken? cancellationToken = null)
        {
            return base.RenameFileAsync(filePath, newName, cancellationToken);
        }

        public override Task RenameFolderAsync(string folderPath, string newName, CancellationToken? cancellationToken = null)
        {
            return base.RenameFolderAsync(folderPath, newName, cancellationToken);
        }

        public override async Task<FsFileProviderType> GetFsFileProviderType(string filePath)
        {
            var drives = await GetDrivesAsync();

            // ToDo: How to get it from the path
            if (IsFsFileProviderInternal(filePath, drives))
            {
                return FsFileProviderType.InternalMemory;
            }
            else if (IsFsFileProviderExternal(filePath, drives))
            {
                return FsFileProviderType.ExternalMemory;
            }
            else
                throw new Exception($"Unknown file provider for path: {filePath}");
        }

        private bool IsFsFileProviderInternal(string filePath, List<FsArtifact> drives)
        {
            var internalDrive = drives?.FirstOrDefault(d => d.ProviderType == FsFileProviderType.InternalMemory);
            if (string.IsNullOrWhiteSpace(filePath) || internalDrive?.FullPath == null)
            {
                return false;
            }
            else if (filePath.StartsWith(internalDrive.FullPath))
            {
                return true;
            }

            return false;
        }

        private bool IsFsFileProviderExternal(string filePath, List<FsArtifact> drives)
        {
            var externalDrives = drives?.Where(d => d.ProviderType == FsFileProviderType.ExternalMemory).ToList();
            if (string.IsNullOrWhiteSpace(filePath) || externalDrives is null || !externalDrives.Any())
            {
                return false;
            }

            foreach (var externalDrive in externalDrives)
            {
                if (externalDrive?.FullPath != null && filePath.StartsWith(externalDrive.FullPath))
                {
                    return true;
                }
            }

            return false;
        }

    }
}
