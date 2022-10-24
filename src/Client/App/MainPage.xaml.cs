using Functionland.FxFiles.Client.App.Extensions;

using Microsoft.Extensions.FileProviders;

#if WINDOWS
using Microsoft.UI;
using WinRT.Interop;
using Microsoft.UI.Windowing;
#endif

#if ANDROID
using Android.Widget;
#endif

namespace Functionland.FxFiles.Client.App;

public partial class MainPage
{
    public MainPage()
    {
        InitializeComponent();

        Microsoft.Maui.Handlers.WindowHandler.Mapper.AppendToMapping(nameof(IWindow), (handler, view) =>
        {
#if WINDOWS
            var nativeWindow = handler.PlatformView;
            nativeWindow.Activate();

            var hWnd = WindowNative.GetWindowHandle(nativeWindow);
            var windowId = Win32Interop.GetWindowIdFromWindow(hWnd);
            var appWindow = AppWindow.GetFromWindowId(windowId);
            var titleBar = appWindow.TitleBar;
            appWindow.Title = "FxFiles";

            if (titleBar is not null)
            {
                if (AppInfo.Current.RequestedTheme == AppTheme.Dark)
                {
                    titleBar.ForegroundColor = Windows.UI.Color.FromArgb(1, 255, 255, 255);
                    titleBar.BackgroundColor = Windows.UI.Color.FromArgb(1, 33, 37, 41);

                    titleBar.InactiveForegroundColor = Windows.UI.Color.FromArgb(1, 255, 255, 255);
                    titleBar.InactiveBackgroundColor = Windows.UI.Color.FromArgb(1, 33, 37, 41);

                    titleBar.ButtonForegroundColor = Windows.UI.Color.FromArgb(1, 255, 255, 255);
                    titleBar.ButtonBackgroundColor = Windows.UI.Color.FromArgb(1, 33, 37, 41);
                    titleBar.ButtonPressedForegroundColor = Windows.UI.Color.FromArgb(1, 255, 255, 255);
                    titleBar.ButtonPressedBackgroundColor = Windows.UI.Color.FromArgb(1, 55, 62, 69);
                    titleBar.ButtonHoverForegroundColor = Windows.UI.Color.FromArgb(1, 255, 255, 255);
                    titleBar.ButtonHoverBackgroundColor = Windows.UI.Color.FromArgb(1, 77, 87, 97);

                    titleBar.ButtonInactiveForegroundColor = Windows.UI.Color.FromArgb(1, 255, 255, 255);
                    titleBar.ButtonInactiveBackgroundColor = Windows.UI.Color.FromArgb(1, 33, 37, 41);
                }
                else
                {
                    titleBar.ForegroundColor = Microsoft.UI.Colors.Black;
                    titleBar.BackgroundColor = Microsoft.UI.Colors.White;

                    titleBar.InactiveForegroundColor = Microsoft.UI.Colors.Black;
                    titleBar.InactiveBackgroundColor = Microsoft.UI.Colors.White;

                    titleBar.ButtonForegroundColor = Microsoft.UI.Colors.Black;
                    titleBar.ButtonBackgroundColor = Microsoft.UI.Colors.White;
                    titleBar.ButtonPressedForegroundColor = Microsoft.UI.Colors.Black;
                    titleBar.ButtonPressedBackgroundColor = Microsoft.UI.Colors.Gray;
                    titleBar.ButtonHoverForegroundColor = Microsoft.UI.Colors.Black;
                    titleBar.ButtonHoverBackgroundColor = Microsoft.UI.Colors.LightGray;

                    titleBar.ButtonInactiveForegroundColor = Microsoft.UI.Colors.Black;
                    titleBar.ButtonInactiveBackgroundColor = Microsoft.UI.Colors.White;
                }
            }
#endif
        });

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

public class FsBlazorWebView : BlazorWebView
{
    public override IFileProvider CreateFileProvider(string contentRootDir)
    {
        var baseFileProvider = base.CreateFileProvider(contentRootDir);
        var fsFileProviderDependency = ServiceExtention.GetRequiredService<FsFileProviderDependency>();
        
        return new FsFileProvider(baseFileProvider, fsFileProviderDependency);
    }
}
