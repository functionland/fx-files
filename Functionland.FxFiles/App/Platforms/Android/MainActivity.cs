using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Widget;

using AndroidX.Core.App;

using Microsoft.Extensions.Localization;

namespace Functionland.FxFiles.App.Platforms.Android;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
    protected override void OnCreate(Bundle? savedInstanceState)
    {
        if (!PermissionUtils.CheckStoragePermission())
        {
            PermissionUtils.RequestStoragePermission();
        }

        base.OnCreate(savedInstanceState);
    }
    
    protected override void OnActivityResult(int requestCode, Result resultCode, Intent? data)
    {
        if (requestCode == PermissionUtils.StoragePermissionRequestCode)
        {
            if (!PermissionUtils.CheckStoragePermission())
            {
                Toast.MakeText(this, "Allow permission for storage access!", ToastLength.Long)?.Show();
            }
        }

        base.OnActivityResult(requestCode, resultCode, data);
    }
}

