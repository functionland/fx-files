using Android.App;
using Android.Runtime;

[assembly: UsesPermission(Android.Manifest.Permission.Internet)]
[assembly: UsesPermission(Android.Manifest.Permission.AccessNetworkState)]
[assembly: UsesPermission(Android.Manifest.Permission.WriteExternalStorage)]
[assembly: UsesPermission(Android.Manifest.Permission.ReadExternalStorage)]
[assembly: UsesPermission(Android.Manifest.Permission.ManageExternalStorage)]
[assembly: UsesPermission(Android.Manifest.Permission.MediaContentControl)]

namespace Functionland.FxFiles.App.Platforms.Android;

[Application(
#if DEBUG
    UsesCleartextTraffic = true
#endif
)]
public class MainApplication : MauiApplication
{
    public MainApplication(IntPtr handle, JniHandleOwnership ownership)
        : base(handle, ownership)
    {
    }

    protected override MauiApp CreateMauiApp() => MauiProgram
        .CreateMauiAppBuilder()
        .Build();
}
