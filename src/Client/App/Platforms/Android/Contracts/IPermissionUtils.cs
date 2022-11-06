using Android.App;
using Android.Content;

namespace Functionland.FxFiles.Client.App.Platforms.Android.Contracts
{
    public interface IPermissionUtils
    {
        int StoragePermissionRequestCode { get; set; }
        TaskCompletionSource<bool>? GetPermissionTask { get; set; }

        Task<bool> CheckReadStoragePermissionAsync(string path);
        Task<bool> CheckWriteStoragePermissionAsync(string filepath = null);
        Task OnPermissionResult(Result resultCode, Intent? data);
        Task RequestStoragePermission(string filepath = null);

    }
}