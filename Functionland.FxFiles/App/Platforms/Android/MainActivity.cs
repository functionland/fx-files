using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Widget;

using android = Android;

namespace Functionland.FxFiles.App.Platforms.Android;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);

        if (!CheckStoragePermission())
        {
            RequestStoragePermission();
        }
    }

    protected override void OnActivityResult(int requestCode, Result resultCode, Intent? data)
    {
        base.OnActivityResult(requestCode, resultCode, data);
        if (requestCode == 2296)
        {
            if (!CheckStoragePermission())
            {
                Toast.MakeText(this, "Allow permission for storage access!", ToastLength.Long).Show();
            }
        }
    }
    private void RequestStoragePermission()
    {
        Intent intent = new Intent(android.Provider.Settings.ActionManageAppAllFilesAccessPermission);
        intent.AddCategory("android.intent.category.DEFAULT");
        intent.SetData(android.Net.Uri.Parse($"package:{MauiApplication.Current.OpPackageName}"));
        StartActivityForResult(intent, 2296);
    }

    public bool CheckStoragePermission()
    {
        if (android.OS.Environment.IsExternalStorageManager)
        {
            return true;
        }
        return false;
    }
}

