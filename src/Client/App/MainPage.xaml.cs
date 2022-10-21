using Functionland.FxFiles.Client.Shared.Utils;
using Microsoft.Extensions.FileProviders;

namespace Functionland.FxFiles.Client.App;

public partial class MainPage
{
    public MainPage()
    {
        InitializeComponent();

        BlazorWebViewHandler.BlazorWebViewMapper.AppendToMapping("CustomBlazorWebViewMapper", (handler, view) =>
        {
#if ANDROID
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
    protected override bool OnBackButtonPressed()
    {
        var backButtonService = MauiApplication.Current.Services.GetRequiredService<IGoBackService>();
        if (backButtonService?.GoBackAsync != null)
        {
            backButtonService.GoBackAsync().GetAwaiter();
        }

        return backButtonService?.CanGoBack ?? base.OnBackButtonPressed();
    }
#endif

}

public class FsBlazorWebView : BlazorWebView
{
    public override IFileProvider CreateFileProvider(string contentRootDir)
    {
        var baseFileProvider = base.CreateFileProvider(contentRootDir);
        var fulaFileService = FsResolver.Resolve<IFulaFileService>(); ;
        var localDeviceFileService = FsResolver.Resolve<ILocalDeviceFileService>();

        return new FsFileProvider(baseFileProvider, localDeviceFileService, fulaFileService);
    }
}
