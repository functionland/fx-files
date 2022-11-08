using Android.Content;
using Android.OS.Storage;
using android = Android;

namespace Functionland.FxFiles.Client.App.Platforms.Android.PermissionsUtility;

public partial class Android11andAbovePermissionUtils : PermissionUtils
{
    [AutoInject] public IFileCacheService FileCacheService { get; set; } = default!;
    public override int StoragePermissionRequestCode { get; set; } = 2296;

    public override Task<bool> CheckReadStoragePermissionAsync(string path)
    {
        return CheckWriteStoragePermissionAsync(path);
    }

    public override async Task<bool> CheckWriteStoragePermissionAsync(string filepath = null)
    {
        if (!string.IsNullOrWhiteSpace(filepath) && IsCacheFile(filepath))
        {
            return true;
        }

        return  android.OS.Environment.IsExternalStorageManager;
    }

    private bool IsCacheFile(string filepath)
    {
        return filepath.StartsWith(FileCacheService.GetAppCacheDirectory());
    }
}

