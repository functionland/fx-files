using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Functionland.FxFiles.Client.Shared.Services.Common;
using Prism.Events;

namespace Functionland.FxFiles.Client.App.Platforms.Android;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = false, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize, Exported = true)]
[IntentFilter(new[] { Intent.ActionView },
    DataHost = "*",
    DataSchemes = new[] { "file", "content" },
    Categories = new[] { Intent.ActionView, Intent.CategoryDefault, Intent.CategoryBrowsable },
    DataMimeTypes = new[]
    {
        "application/zip", 

        //TODO: Currently we don't have viewer for zip
        //"application/x-rar-compressed", 
       
        //TODO: Currently, the video player we have does not have the ability to run the video file outside the program, and in some cases, it does not work.
        //"video/*" 
       
        "text/plain",
        "image/jpg","image/jpeg","image/png","image/gif","image/bmp","image/svg","image/svg+xml","image/x-ms-bmp","image/webp","image/jfif","image/ico",

    })]
public class FileViewerActivity : MainActivity
{
    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);

        var appStateStore = MauiApplication.Current.Services.GetRequiredService<IAppStateStore>();
        var eventAggregator = MauiApplication.Current.Services.GetRequiredService<IEventAggregator>();

        if (string.IsNullOrWhiteSpace(Intent?.DataString))
            return;

        appStateStore.IntentFileUrl = Intent.DataString;
        eventAggregator.GetEvent<IntentReceiveEvent>().Publish(new IntentReceiveEvent());
    }
}