using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Widget;

namespace Functionland.FxFiles.App.Platforms.Android;

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
        base.OnActivityResult(requestCode, resultCode, data);
        if (requestCode == 2296)
        {
            if (!PermissionUtils.CheckStoragePermission())
            {
                AlertDialog.Builder builder = new AlertDialog.Builder(this);

                builder.SetCancelable(false);
                builder.SetMessage("The app does not have critical permissions needed to run. Please check your permissions settings.");
                builder.SetNeutralButton("QUIT", (sent, args) =>
                {
                    MoveTaskToBack(true);
                });
                AlertDialog? dialog = builder.Create();
                dialog?.Show();
            }
        }
    }
}

