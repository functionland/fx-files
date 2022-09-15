using Android.Content;
using android = Android;
using Microsoft.Maui.Controls.PlatformConfiguration;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui.ApplicationModel;

namespace Functionland.FxFiles.App.Platforms.Android
{
    public static class PermissionUtils
    {
        public static void RequestStoragePermission()
        {
            Intent intent = new Intent(android.Provider.Settings.ActionManageAppAllFilesAccessPermission);
            intent.AddCategory("android.intent.category.DEFAULT");
            intent.SetData(android.Net.Uri.Parse($"package:{MauiApplication.Current.OpPackageName}"));
            Platform.CurrentActivity?.StartActivityForResult(intent, 2296);
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
