using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Widget;
using Functionland.FxFiles.Client.App.Platforms.Android.PermissionsUtility;

namespace Functionland.FxFiles.Client.App.Platforms.Android;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{


    protected override async void OnCreate(Bundle? savedInstanceState)
    {
        try
        {
            base.OnCreate(savedInstanceState);

            if (!await PermissionUtils.CheckStoragePermissionAsync())
            {
                await PermissionUtils.RequestStoragePermissionAsync();
            }
        }
        catch (Exception ex)
        {
            throw;
        }
    }

    protected override async void OnActivityResult(int requestCode, Result resultCode, Intent? data)
    {
        try
        {
            if (requestCode == PermissionUtils.StoragePermissionRequestCode)
            {
                if (!await PermissionUtils.CheckStoragePermissionAsync())
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
        catch (Exception ex)
        {
            PermissionUtils.GetPermissionTask?.SetResult(false);
        }
    }


}

