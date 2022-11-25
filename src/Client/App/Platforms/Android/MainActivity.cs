using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using CommunityToolkit.Maui.MediaElement;
using Functionland.FxFiles.Client.App.Platforms.Android.Contracts;
using Functionland.FxFiles.Client.App.Views;

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

            if (!await permissionUtils.CheckWriteStoragePermissionAsync())
            {
                await permissionUtils.RequestStoragePermission();
            }
        }
        catch
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
        catch
        {
            throw;
        }
    }

    protected override void OnStop()
    {
        NativeVideoViewer.Current?.TogglePlay(MediaElementState.Paused);
        base.OnStop();
    }
}

