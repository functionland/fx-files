using Android.Content;
using Android.OS.Storage;
using Functionland.FxFiles.Client.App.Platforms.Android.PermissionsUtility;
using Functionland.FxFiles.Client.Shared.Components.Modal;
using Functionland.FxFiles.Client.Shared.Enums;
using Functionland.FxFiles.Client.Shared.Exceptions;
using Functionland.FxFiles.Client.Shared.Models;
using Functionland.FxFiles.Client.Shared.Resources;

namespace Functionland.FxFiles.Client.App.Platforms.Android.Implementations;

public partial class AndroidFileService : LocalDeviceFileService
{
    public override async Task<FsArtifact> CreateFileAsync(string path, Stream stream, CancellationToken? cancellationToken = null)
    {
        if (!await PermissionUtils.CheckStoragePermissionAsync())
        {
            await PermissionUtils.RequestStoragePermissionAsync();

            var StoragePermissionResult = await PermissionUtils.GetPermissionTask!.Task;
            if (!StoragePermissionResult)
            {
                throw new UnableAccessToStorageException(StringLocalizer.GetString(AppStrings.UnableToAccessToStorage));
            }
        }

        return await base.CreateFileAsync(path, stream, cancellationToken);
    }

    public override async Task<List<FsArtifact>> CreateFilesAsync(IEnumerable<(string path, Stream stream)> files, CancellationToken? cancellationToken = null)
    {
        if (!await PermissionUtils.CheckStoragePermissionAsync())
        {
            await PermissionUtils.RequestStoragePermissionAsync();

            var StoragePermissionResult = await PermissionUtils.GetPermissionTask!.Task;
            if (!StoragePermissionResult)
            {
                throw new UnableAccessToStorageException(StringLocalizer.GetString(AppStrings.UnableToAccessToStorage));
            }
        }

        return await base.CreateFilesAsync(files, cancellationToken);
    }

    public override async Task<FsArtifact> CreateFolderAsync(string path, string folderName, CancellationToken? cancellationToken = null)
    {
        if (!await PermissionUtils.CheckStoragePermissionAsync())
        {
            await PermissionUtils.RequestStoragePermissionAsync();

            var StoragePermissionResult = await PermissionUtils.GetPermissionTask!.Task;
            if (!StoragePermissionResult)
            {
                throw new UnableAccessToStorageException(StringLocalizer.GetString(AppStrings.UnableToAccessToStorage));
            }
        }

        return await base.CreateFolderAsync(path, folderName, cancellationToken);
    }

    public override async Task<Stream> GetFileContentAsync(string filePath, CancellationToken? cancellationToken = null)
    {
        if (!await PermissionUtils.CheckStoragePermissionAsync())
        {
            await PermissionUtils.RequestStoragePermissionAsync();

            var StoragePermissionResult = await PermissionUtils.GetPermissionTask!.Task;
            if (!StoragePermissionResult)
            {
                throw new UnableAccessToStorageException(StringLocalizer.GetString(AppStrings.UnableToAccessToStorage));
            }
        }

        return await base.GetFileContentAsync(filePath, cancellationToken);
    }

    public override async IAsyncEnumerable<FsArtifact> GetArtifactsAsync(string? path = null, string? searchText = null, CancellationToken? cancellationToken = null)
    {
        if (path is null && string.IsNullOrWhiteSpace(searchText))
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

    public override async Task<FsArtifact?> GetArtifactAsync(string? path = null, CancellationToken? cancellationToken = null)
    {
        return await base.GetArtifactAsync(path, cancellationToken);
    }

    public override async Task MoveArtifactsAsync(IList<FsArtifact> artifacts, string destination, bool overwrite = false, Action<ProgressInfo>? onProgress = null, CancellationToken? cancellationToken = null)
    {
        if (!await PermissionUtils.CheckStoragePermissionAsync())
        {
            await PermissionUtils.RequestStoragePermissionAsync();

            var StoragePermissionResult = await PermissionUtils.GetPermissionTask!.Task;
            if (!StoragePermissionResult)
            {
                throw new UnableAccessToStorageException(StringLocalizer.GetString(AppStrings.UnableToAccessToStorage));
            }
        }

        await base.MoveArtifactsAsync(artifacts, destination, overwrite, onProgress, cancellationToken);
    }

    public override async Task CopyArtifactsAsync(IList<FsArtifact> artifacts, string destination, bool overwrite = false, Action<ProgressInfo>? onProgress = null, CancellationToken? cancellationToken = null)
    {
        if (!await PermissionUtils.CheckStoragePermissionAsync())
        {
            await PermissionUtils.RequestStoragePermissionAsync();

            var StoragePermissionResult = await PermissionUtils.GetPermissionTask!.Task;
            if (!StoragePermissionResult)
            {
                throw new UnableAccessToStorageException(StringLocalizer.GetString(AppStrings.UnableToAccessToStorage));
            }
        }

        await base.CopyArtifactsAsync(artifacts, destination, overwrite, onProgress, cancellationToken);
    }

    public override async Task RenameFileAsync(string filePath, string newName, CancellationToken? cancellationToken = null)
    {
        if (!await PermissionUtils.CheckStoragePermissionAsync())
        {
            await PermissionUtils.RequestStoragePermissionAsync();

            var StoragePermissionResult = await PermissionUtils.GetPermissionTask!.Task;
            if (!StoragePermissionResult)
            {
                throw new UnableAccessToStorageException(StringLocalizer.GetString(AppStrings.UnableToAccessToStorage));
            }
        }

        await base.RenameFileAsync(filePath, newName, cancellationToken);
    }

    public override async Task RenameFolderAsync(string folderPath, string newName, CancellationToken? cancellationToken = null)
    {
        if (!await PermissionUtils.CheckStoragePermissionAsync())
        {
            await PermissionUtils.RequestStoragePermissionAsync();

            var StoragePermissionResult = await PermissionUtils.GetPermissionTask!.Task;
            if (!StoragePermissionResult)
            {
                throw new UnableAccessToStorageException(StringLocalizer.GetString(AppStrings.UnableToAccessToStorage));
            }
        }

        await base.RenameFolderAsync(folderPath, newName, cancellationToken);
    }

    public override async Task DeleteArtifactsAsync(IList<FsArtifact> artifacts, Action<ProgressInfo>? onProgress = null, CancellationToken? cancellationToken = null)
    {
        if (!await PermissionUtils.CheckStoragePermissionAsync())
        {
            await PermissionUtils.RequestStoragePermissionAsync();

            var StoragePermissionResult = await PermissionUtils.GetPermissionTask!.Task;
            if (!StoragePermissionResult)
            {
                throw new UnableAccessToStorageException(StringLocalizer.GetString(AppStrings.UnableToAccessToStorage));
            }
        }

        await base.DeleteArtifactsAsync(artifacts, onProgress, cancellationToken);
    }

    public override async Task<List<FsArtifactChanges>> CheckPathExistsAsync(List<string?> paths, CancellationToken? cancellationToken = null)
    {
        return await base.CheckPathExistsAsync(paths, cancellationToken);
    }

    public override async Task<List<FsArtifact>> GetDrivesAsync()
    {
        var storageManager = MauiApplication.Current.GetSystemService(Context.StorageService) as StorageManager;
        if (storageManager is null)
        {
            throw new UnableAccessToStorageException(StringLocalizer.GetString(AppStrings.UnableToLoadStorageManager));
        }

        var storageVolumes = storageManager.StorageVolumes.ToList();

        var drives = new List<FsArtifact>();
        foreach (var storage in storageVolumes)
        {
            if (storage is null || storage?.Directory is null)
                continue;

            var lastModifiedUnixFormat = storage.Directory?.LastModified() ?? 0;
            var lastModifiedDateTime = lastModifiedUnixFormat == 0
                                                        ? DateTimeOffset.Now
                                                        : DateTimeOffset.FromUnixTimeMilliseconds(lastModifiedUnixFormat);
            var fullPath = storage.Directory?.Path;
            var capacity = storage.Directory?.FreeSpace;
            var size = storage.Directory?.TotalSpace;

            if (storage.IsPrimary)
            {
                var internalFileName = StringLocalizer.GetString(AppStrings.InternalStorageName);
                drives.Add(new FsArtifact(fullPath, internalFileName, FsArtifactType.Drive, FsFileProviderType.InternalMemory)
                {
                    Capacity = capacity,
                    Size = size,
                    LastModifiedDateTime = lastModifiedDateTime,
                });
            }
            else
            {
                var sDCardName = storage.MediaStoreVolumeName ?? string.Empty;
                var externalFileName = StringLocalizer.GetString(AppStrings.SDCardName, sDCardName);
                drives.Add(new FsArtifact(fullPath, externalFileName, FsArtifactType.Drive, FsFileProviderType.ExternalMemory)
                {
                    Capacity = capacity,
                    Size = size,
                    LastModifiedDateTime = lastModifiedDateTime,
                });
            }
        }

        return drives;
    }

    public override async Task<FsArtifactType?> GetFsArtifactTypeAsync(string path)
    {
        var isDrive = await FsArtifactIsDriveAsync(path);

        if (isDrive)
        {
            return FsArtifactType.Drive;
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
            return null;
        }
    }

    public override async Task<FsFileProviderType> GetFsFileProviderTypeAsync(string filePath)
    {
        var drives = await GetDrivesAsync();

        if (IsFsFileProviderInternal(filePath, drives))
        {
            return FsFileProviderType.InternalMemory;
        }
        else if (IsFsFileProviderExternal(filePath, drives))
        {
            return FsFileProviderType.ExternalMemory;
        }
        else
            throw new UnknownFsFileProviderException(StringLocalizer.GetString(AppStrings.UnknownFsFileProviderException, filePath));
    }

    private static bool IsFsFileProviderInternal(string filePath, List<FsArtifact> drives)
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

    private static bool IsFsFileProviderExternal(string filePath, List<FsArtifact> drives)
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

    private async Task<bool> FsArtifactIsDriveAsync(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
            return false;

        var drives = await GetDrivesAsync();
        var drivesPath = drives
                                    .Where(drive => String.IsNullOrWhiteSpace(drive.FullPath) is false)
                                    .Select(drive => drive.FullPath)
                                    .ToList();

        var IsDrive = drivesPath.Any(drivePath => drivePath!.Equals(path));
        if (IsDrive)
            return true;

        return false;
    }
}
