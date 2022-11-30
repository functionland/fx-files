using Android.Content;
using Android.OS.Storage;

using Functionland.FxFiles.Client.App.Platforms.Android.Contracts;
using Functionland.FxFiles.Client.App.Platforms.Android.PermissionsUtility;
using Functionland.FxFiles.Client.Shared.Components.Modal;
using Functionland.FxFiles.Client.Shared.Enums;
using Functionland.FxFiles.Client.Shared.Exceptions;
using Functionland.FxFiles.Client.Shared.Models;
using Functionland.FxFiles.Client.Shared.Resources;

using android = Android;

namespace Functionland.FxFiles.Client.App.Platforms.Android.Implementations;

public abstract partial class AndroidFileService : LocalDeviceFileService
{
    [AutoInject] public IPermissionUtils PermissionUtils { get; set; } = default!;
    private List<FsArtifact>? _allDrives = null;

    public override async Task<FsArtifact> CreateFileAsync(string path, Stream stream, CancellationToken? cancellationToken = null)
    {
        await GetWritePermission(path);

        return await base.CreateFileAsync(path, stream, cancellationToken);
    }

    public override async Task<List<FsArtifact>> CreateFilesAsync(IEnumerable<(string path, Stream stream)> files, CancellationToken? cancellationToken = null)
    {
        await GetWritePermission(files.Select(f => f.path));

        return await base.CreateFilesAsync(files, cancellationToken);
    }

    public override async Task<FsArtifact> CreateFolderAsync(string path, string folderName, CancellationToken? cancellationToken = null)
    {
        await GetWritePermission(path);

        return await base.CreateFolderAsync(path, folderName, cancellationToken);
    }

    public override async Task<Stream> GetFileContentAsync(string filePath, CancellationToken? cancellationToken = null)
    {
        await GetReadPermission(filePath);

        return await base.GetFileContentAsync(filePath, cancellationToken);
    }

    public override async IAsyncEnumerable<FsArtifact> GetArtifactsAsync(string? path = null, CancellationToken? cancellationToken = null)
    {
        if (path is null)
        {
            var drives = GetDrives();
            foreach (var drive in drives)
            {
                yield return drive;
            }
            yield break;
        }

        await foreach (var artifact in base.GetArtifactsAsync(path, cancellationToken))
        {
            yield return artifact;
        }
    }

    public override async Task<FsArtifact?> GetArtifactAsync(string? path = null, CancellationToken? cancellationToken = null)
    {
        return await base.GetArtifactAsync(path, cancellationToken);
    }

    public override async Task<List<(FsArtifact artifact, Exception exception)>> MoveArtifactsAsync(
        IList<FsArtifact> artifacts,
        string destination,
        Func<FsArtifact, Task<bool>>? onShouldOverwrite = null,
        Func<ProgressInfo, Task>? onProgress = null,
        CancellationToken? cancellationToken = null)
    {
        await GetReadPermission(artifacts.Select(f => f.FullPath));
        await GetWritePermission(destination);

        return await base.MoveArtifactsAsync(artifacts, destination, onShouldOverwrite, onProgress, cancellationToken);
    }

    public override async Task<List<(FsArtifact artifact, Exception exception)>> CopyArtifactsAsync(
        IList<FsArtifact> artifacts,
        string destination,
        Func<FsArtifact, Task<bool>>? onShouldOverwrite = null,
        Func<ProgressInfo, Task>? onProgress = null,
        CancellationToken? cancellationToken = null)
    {
        await GetReadPermission(artifacts.Select(f => f.FullPath));
        await GetWritePermission(destination);

        return await base.CopyArtifactsAsync(artifacts, destination, onShouldOverwrite, onProgress, cancellationToken);
    }

    public override async Task CopyFileAsync(FsArtifact artifact, string destinationFullPath, Func<FsArtifact, Task<bool>>? onShouldOverwrite = null, Func<ProgressInfo, Task>? onProgress = null, CancellationToken? cancellationToken = null)
    {
        await GetReadPermission(artifact.FullPath);
        await GetWritePermission(destinationFullPath);

        await base.CopyFileAsync(artifact, destinationFullPath, onShouldOverwrite, onProgress, cancellationToken);
    }

    public override async Task RenameFileAsync(string filePath, string newName, CancellationToken? cancellationToken = null)
    {
        await GetWritePermission(filePath);

        await base.RenameFileAsync(filePath, newName, cancellationToken);
    }

    public override async Task RenameFolderAsync(string folderPath, string newName, CancellationToken? cancellationToken = null)
    {
        await GetWritePermission(folderPath);

        await base.RenameFolderAsync(folderPath, newName, cancellationToken);
    }

    public override async Task DeleteArtifactsAsync(
        IList<FsArtifact> artifacts,
        Func<ProgressInfo, Task>? onProgress = null,
        CancellationToken? cancellationToken = null)
    {
        await GetWritePermission(artifacts.Select(f => f.FullPath));

        await base.DeleteArtifactsAsync(artifacts, onProgress, cancellationToken);
    }

    public override async Task<List<(string Path, bool IsExist)>> CheckPathExistsAsync(IEnumerable<string?> paths,
        CancellationToken? cancellationToken = null)
    {
        return await base.CheckPathExistsAsync(paths, cancellationToken);
    }

    public override List<FsArtifact> GetDrives()
    {
        LazyInitializer.EnsureInitialized(ref _allDrives, LoadDrives);
        return _allDrives;
    }

    private List<FsArtifact> LoadDrives()
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

            if (storage is null || !string.Equals(storage.State, android.OS.Environment.MediaMounted, StringComparison.InvariantCultureIgnoreCase))
                continue;


            var storageDirectory = storage.Directory;

            var lastModifiedUnixFormat = storageDirectory.LastModified();
            var lastModifiedDateTime = lastModifiedUnixFormat == 0
                                                        ? DateTimeOffset.Now
                                                        : DateTimeOffset.FromUnixTimeMilliseconds(lastModifiedUnixFormat);
            var fullPath = storageDirectory.Path;
            var capacity = storageDirectory.FreeSpace;
            var size = storageDirectory.TotalSpace;

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
                string sdCardName;

                sdCardName = storage.MediaStoreVolumeName ?? string.Empty;

                var externalFileName = StringLocalizer.GetString(AppStrings.SDCardName, sdCardName);
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

    public override FsArtifactType GetFsArtifactType(string path)
    {
        try
        {
            var isDrive = IsDrive(path);

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
                ExceptionHandler.Track(new InvalidOperationException($"File type is not valid. path: '{path}'"));
                throw new DomainLogicException(StringLocalizer.GetString(nameof(AppStrings.PathNotFound), path));
            }
        }
        catch (IOException ex)
        {
            throw new KnownIOException(ex.Message, ex);
        }
        catch (UnauthorizedAccessException ex)
        {
            throw new UnauthorizedException(ex.Message, ex);
        }
    }

    public override FsFileProviderType GetFsFileProviderType(string filePath)
    {
        var drives = GetDrives();

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

    public override string GetShowablePath(string artifactPath)
    {
        if (artifactPath is null)
            throw new ArtifactPathNullException(nameof(artifactPath));

        var providerType = GetFsFileProviderType(artifactPath);
        var drives = GetDrives();

        var currentDrivePath = drives.Where(d => d.ProviderType == providerType)
                                     .Select(d => d.FullPath).FirstOrDefault();

        if (currentDrivePath is null)
            throw new Exception("No drive found!");

        //ToDo: Replace is dangrous here. It replaces all the matching strings inside artifactPath. (Anything that matches "stoarge/emulated/0" and SD card's specific name).
        return artifactPath.Replace(currentDrivePath, providerType == FsFileProviderType.InternalMemory
                                                                    ? StringLocalizer.GetString(AppStrings.InternalStorageName)
                                                                    : StringLocalizer.GetString(AppStrings.SDCard));
    }
    protected abstract Task GetWritePermission(string path = null);

    protected abstract Task GetWritePermission(IEnumerable<string> paths = null);

    protected abstract Task GetReadPermission(string path = null);

    protected abstract Task GetReadPermission(IEnumerable<string> paths = null);


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

    private bool IsDrive(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
            return false;

        var drives = GetDrives();
        var drivesPath = drives
                                    .Where(drive => String.IsNullOrWhiteSpace(drive.FullPath) is false)
                                    .Select(drive => drive.FullPath)
                                    .ToList();

        var IsDrive = drivesPath.Any(drivePath => drivePath!.Equals(path));
        if (IsDrive)
            return true;

        return false;
    }

    protected override long CalculateDriveSize(string drivePath, CancellationToken? cancellation = null)
    {
        var drives = LoadDrives();
        var targetDrive = drives.FirstOrDefault(drive => drive.FullPath.Equals(drivePath, StringComparison.OrdinalIgnoreCase));

        if (targetDrive is null)
            throw new InvalidOperationException("No drive found given the current path.");

        return targetDrive.Size ?? 0;
    }
}
