using Microsoft.UI.Xaml;

namespace Functionland.FxFiles.App.Platforms.Windows;

public partial class App
{
    public App()
    {
        InitializeComponent();
        UnhandledException += OnUnhandledException;
    }

    private void OnUnhandledException(object sender, Microsoft.UI.Xaml.UnhandledExceptionEventArgs e)
    {
        e.Handled = true;
    }

    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiAppBuilder().Build();
}
