using Android.Content;

using AndroidX.Activity.Result;

using android = Android;

namespace Functionland.FxFiles.App.Platforms.Android
{
    public static class PermissionUtils
    {
        public static TaskCompletionSource<bool>? GetPermissionTask { get; set; }
        public static int StoragePermissionRequestCode { get; set; } = 2296;
        public static void RequestStoragePermission()
        {
            GetPermissionTask = new TaskCompletionSource<bool>();
            Intent intent = new Intent(android.Provider.Settings.ActionManageAppAllFilesAccessPermission);
            intent.AddCategory("android.intent.category.DEFAULT");
            intent.SetData(android.Net.Uri.Parse($"package:{MauiApplication.Current.OpPackageName}"));
            Platform.CurrentActivity?.StartActivityForResult(intent, StoragePermissionRequestCode);
        }

        public static bool CheckStoragePermission()
        {
            if (android.OS.Environment.IsExternalStorageManager)
            {
                return true;
            }
            return false;
        }
    }
}
