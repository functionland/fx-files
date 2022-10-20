using Microsoft.Extensions.FileProviders;
using Functionland.FxFiles.Client.Shared.Resources;
using Microsoft.Maui;
using System.Drawing;

#if ANDROID
using Android.Widget;
#endif

namespace Functionland.FxFiles.Client.App;

public partial class MainPage
{
    public MainPage()
    {
        InitializeComponent();

        BlazorWebViewHandler.BlazorWebViewMapper.AppendToMapping("CustomBlazorWebViewMapper", (handler, view) =>
        {

#if IOS
            handler.PlatformView.BackgroundColor = UIKit.UIColor.Clear;
            handler.PlatformView.Opaque = false;
#endif

#if ANDROID
            handler.PlatformView.SetBackgroundColor(Android.Graphics.Color.Transparent);

            Android.Webkit.WebSettings settings = handler.PlatformView.Settings;

            settings.AllowFileAccessFromFileURLs =
                settings.AllowUniversalAccessFromFileURLs =
                settings.AllowContentAccess =
                settings.AllowFileAccess =
                settings.DatabaseEnabled =
                settings.JavaScriptCanOpenWindowsAutomatically =
                settings.DomStorageEnabled = true;

#if DEBUG
            settings.MixedContentMode = Android.Webkit.MixedContentHandling.AlwaysAllow;
#endif

            settings.BlockNetworkLoads =
            settings.BlockNetworkImage = false;
#endif
        });

    }


#if ANDROID
    long lastPress;
    protected override bool OnBackButtonPressed()
    {
        var backButtonService = MauiApplication.Current.Services.GetRequiredService<IGoBackService>();
        if (backButtonService?.CanGoBack is true && backButtonService?.GoBackAsync is not null)
        {
            backButtonService.GoBackAsync().GetAwaiter();
            return true;
        }
        else if (backButtonService?.CanGoBack is true && backButtonService?.CanExitApp is true)
        {
            long currentTime = DateTimeOffset.UtcNow.Ticks / TimeSpan.TicksPerMillisecond;
            if (currentTime - lastPress > 5000)
            {
                var context = MauiApplication.Current.ApplicationContext;
                Toast.MakeText(context, "Press back again to exit", ToastLength.Long)?.Show();
                lastPress = currentTime;
                return true;
            }

            return false;
        }

        return true;
    }
#endif

}

public class FxBlazorWebView : BlazorWebView
{
    public override IFileProvider CreateFileProvider(string contentRootDir)
    {
        var baseFileProvider = base.CreateFileProvider(contentRootDir);
        return new FxFileProvider(baseFileProvider);
    }
}
