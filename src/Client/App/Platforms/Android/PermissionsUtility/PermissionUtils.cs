using Android.Content;

using android = Android;

namespace Functionland.FxFiles.Client.App.Platforms.Android.PermissionsUtility
{
    public static class PermissionUtils
    {
        public static TaskCompletionSource<bool>? GetPermissionTask { get; set; }
        public static int StoragePermissionRequestCode { get; set; } = 2296;
        public static async Task RequestStoragePermissionAsync()
        {
            if (android.OS.Build.VERSION.SdkInt >= android.OS.BuildVersionCodes.R)
            {
                GetPermissionTask = new TaskCompletionSource<bool>();
                Intent intent = new Intent(android.Provider.Settings.ActionManageAppAllFilesAccessPermission);
                intent.AddCategory("android.intent.category.DEFAULT");
                intent.SetData(android.Net.Uri.Parse($"package:{MauiApplication.Current.OpPackageName}"));
                Platform.CurrentActivity?.StartActivityForResult(intent, StoragePermissionRequestCode);
            }
            else
            {
                GetPermissionTask = new TaskCompletionSource<bool>();
                await Permissions.RequestAsync<FunctionlandPermission>();
                GetPermissionTask.SetResult(await CheckStoragePermissionAsync());
            }
        }

        public static async Task<bool> CheckStoragePermissionAsync()
        {

            if (android.OS.Build.VERSION.SdkInt >= android.OS.BuildVersionCodes.R)
            {
                return android.OS.Environment.IsExternalStorageManager;
            }
            else
            {
                return await Permissions.CheckStatusAsync<FunctionlandPermission>() == PermissionStatus.Granted;
            }
        }
    }
}
