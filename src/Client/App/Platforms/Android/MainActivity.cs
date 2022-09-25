using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Widget;
using Functionland.FxFiles.App.Platforms.Android;

namespace Functionland.FxFiles.Client.App.Platforms.Android;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);

        if (!PermissionUtils.CheckStoragePermission())
        {
            PermissionUtils.RequestStoragePermission();
        }
    }

    protected override void OnActivityResult(int requestCode, Result resultCode, Intent? data)
    {
        if (requestCode == PermissionUtils.StoragePermissionRequestCode)
        {
            if (!PermissionUtils.CheckStoragePermission())
            {
                PermissionUtils.GetPermissionTask?.SetResult(false);
                Toast.MakeText(this, "Allow permission for storage access!", ToastLength.Long)?.Show();
            }
            else
            {
                PermissionUtils.GetPermissionTask?.SetResult(true);
            }
        }

        base.OnActivityResult(requestCode, resultCode, data);
    }


}

