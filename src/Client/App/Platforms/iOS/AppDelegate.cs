using Foundation;
using UIKit;

namespace Functionland.FxFiles.Client.App.Platforms.iOS;

[Register(nameof(AppDelegate))]
public class AppDelegate : MauiUIApplicationDelegate
{
    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiAppBuilder().Build();

    public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
    {
        Thread.Sleep(200);
        return base.FinishedLaunching(application, launchOptions);
    }
}