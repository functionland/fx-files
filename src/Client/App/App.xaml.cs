using CommunityToolkit.Maui.MediaElement;
using Functionland.FxFiles.Client.App.Views;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]

namespace Functionland.FxFiles.Client.App;

public partial class App
{
    private IExceptionHandler ExceptionHandler { get; }
    public IServiceProvider ServiceProvider { get; set; }

    public App(IExceptionHandler exceptionHandler, IServiceProvider serviceProvider)
    {
        InitializeComponent();
        MainPage = new NavigationPage(new MainPage());
        ExceptionHandler = exceptionHandler;
        ServiceProvider = serviceProvider;
    }

    protected override void OnStart()
    {
        base.OnStart();
        _ = Task.Run(async () => { await ServiceProvider.RunAppEvents(); });
    }

    protected override void OnSleep()
    {
        base.OnSleep();
        NativeVideoViewer.Current?.TogglePlay(MediaElementState.Paused);
    }
}
