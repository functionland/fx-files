using Android.App;
using Android.Content;
using Android.OS.Storage;
using android = Android;

namespace Functionland.FxFiles.Client.App.Platforms.Android.PermissionsUtility;

public class Android10PermissionUtils : PermissionUtils
{
    public TaskCompletionSource<bool>? GetPermissionTask { get; set; }
    public override int StoragePermissionRequestCode { get; set; } = 2020;

    public override void RequestStoragePermission(bool isSdCard = false)
    {
        GetPermissionTask = new TaskCompletionSource<bool>();

        if (isSdCard)
        {
            var storageManager = MauiApplication.Current.GetSystemService(Context.StorageService) as StorageManager;
            var volume = storageManager.StorageVolumes.ToList().FirstOrDefault(r => !r.IsPrimary);
            var intent = volume.CreateOpenDocumentTreeIntent();
            Platform.CurrentActivity?.StartActivityForResult(intent, StoragePermissionRequestCode);
        }
        else
        {
            _ = Task.Run(async () =>
            {
                await Permissions.RequestAsync<Android10StoragePermission>();
                GetPermissionTask.SetResult(await CheckStoragePermissionAsync());
            });
        }
    }

    public override async Task<bool> CheckStoragePermissionAsync(string filepath = null)
    {
        if (string.IsNullOrWhiteSpace(filepath))
        {
            return await Permissions.CheckStatusAsync<Android10StoragePermission>() == PermissionStatus.Granted;
        }
        else
        {
            return false;
        }
    }

    public override async Task OnPermissionResult(Result resultCode, Intent? data)
    {
        if (resultCode is Result.Ok)
        {
            var treeUri = data;
            var takeFlags = data!.Flags & (ActivityFlags.GrantReadUriPermission | ActivityFlags.GrantWriteUriPermission);
            MauiApplication.Context?.ContentResolver?.TakePersistableUriPermission(treeUri.Data, takeFlags);
            GetPermissionTask?.SetResult(true);
        }
        else
        {
            GetPermissionTask?.SetResult(false);
        }
    }
}

