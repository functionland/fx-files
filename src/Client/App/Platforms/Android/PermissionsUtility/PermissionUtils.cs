using Android.App;
using Android.Content;
using Android.OS.Storage;
using Android.Widget;
using Functionland.FxFiles.Client.App.Platforms.Android.Contracts;
using android = Android;

namespace Functionland.FxFiles.Client.App.Platforms.Android.PermissionsUtility;

public abstract class PermissionUtils : IPermissionUtils
{
    public TaskCompletionSource<bool>? GetPermissionTask { get; set; }

    public abstract int StoragePermissionRequestCode { get; set; }

    public virtual async Task RequestStoragePermission(string filepath = null)
    {
        GetPermissionTask = new TaskCompletionSource<bool>();
        Intent intent = new Intent(android.Provider.Settings.ActionManageAppAllFilesAccessPermission);
        intent.AddCategory("android.intent.category.DEFAULT");
        intent.SetData(android.Net.Uri.Parse($"package:{MauiApplication.Current.OpPackageName}"));
        Platform.CurrentActivity?.StartActivityForResult(intent, StoragePermissionRequestCode);
    }

    public abstract Task<bool> CheckWriteStoragePermissionAsync(string filepath = null);
    

    public virtual async Task OnPermissionResult(Result resultCode, Intent? data)
    {
        if (!await CheckWriteStoragePermissionAsync())
        {
            GetPermissionTask?.SetResult(false);
            Toast.MakeText(MauiApplication.Context, "Allow permission for storage access!", ToastLength.Long)?.Show();
        }
        else
        {
            GetPermissionTask?.SetResult(true);
        }
    }

    public abstract Task<bool> CheckReadStoragePermissionAsync(string path);
}

