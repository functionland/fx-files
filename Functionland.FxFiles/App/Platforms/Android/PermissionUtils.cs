using Android.Content;
using android = Android;

namespace Functionland.FxFiles.App.Platforms.Android
{
    public static class PermissionUtils
    {
        public static int StoragePermissionRequestCode { get; set; } = 2296;
        public static void RequestStoragePermission()
        {
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
