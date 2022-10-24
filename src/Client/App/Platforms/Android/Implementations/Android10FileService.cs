using Android.Content;
using Android.OS.Storage;
using Android.Provider;
using AndroidX.DocumentFile.Provider;
using Functionland.FxFiles.Client.App.Platforms.Android.PermissionsUtility;
using Functionland.FxFiles.Client.Shared.Components.Modal;
using Functionland.FxFiles.Client.Shared.Enums;
using Functionland.FxFiles.Client.Shared.Exceptions;
using Functionland.FxFiles.Client.Shared.Models;
using Functionland.FxFiles.Client.Shared.Resources;
using Microsoft.Extensions.Localization;
using Microsoft.Maui.Animations;
using android = Android;

namespace Functionland.FxFiles.Client.App.Platforms.Android.Implementations;

public partial class Android10FileService : AndroidFileService
{
    
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

            if (storage is null || !string.Equals(storage.State, android.OS.Environment.MediaMounted, StringComparison.InvariantCultureIgnoreCase))
                continue;

            Java.IO.File storageDirectory;


            if (storage.IsPrimary)
            {
                storageDirectory = android.OS.Environment.ExternalStorageDirectory;
            }
            else
            {
                storageDirectory = new Java.IO.File($@"/storage/{storage.Uuid}");
            }


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
                string sdCardName = storage.Uuid ?? string.Empty;

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

}
