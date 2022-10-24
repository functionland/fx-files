using Android.App;
using Android.Content;

namespace Functionland.FxFiles.Client.App.Platforms.Android.Contracts
{
    public interface IPermissionUtils
    {
        int StoragePermissionRequestCode { get; set; }
        TaskCompletionSource<bool>? GetPermissionTask { get; set; }

        Task<bool> CheckStoragePermissionAsync(string filepath = null);
        Task OnPermissionResult(Result resultCode, Intent? data);
        void RequestStoragePermission(bool isSdCard = false);

    }
}