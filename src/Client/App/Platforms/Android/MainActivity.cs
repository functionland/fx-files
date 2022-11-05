using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Widget;
using Functionland.FxFiles.Client.App.Platforms.Android.Contracts;
using Functionland.FxFiles.Client.App.Platforms.Android.PermissionsUtility;

namespace Functionland.FxFiles.Client.App.Platforms.Android;

[Activity(Theme = "@style/Maui.SplashTheme", SupportsPictureInPicture = true, MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{

    private IPermissionUtils permissionUtils;

    protected override async void OnCreate(Bundle? savedInstanceState)
    {
        try
        {

            base.OnCreate(savedInstanceState);

            permissionUtils = MauiApplication.Current.Services.GetRequiredService<IPermissionUtils>();

            if (!await permissionUtils.CheckStoragePermissionAsync())
            {
                await permissionUtils.RequestStoragePermission();
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
            if (requestCode == permissionUtils.StoragePermissionRequestCode)
            {
                await permissionUtils.OnPermissionResult(resultCode, data);
            }

            base.OnActivityResult(requestCode, resultCode, data);

        }
        catch (Exception ex)
        {

        }
    }


}

