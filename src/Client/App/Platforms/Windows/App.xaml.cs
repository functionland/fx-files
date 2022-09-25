using Microsoft.UI.Xaml;

namespace Functionland.FxFiles.Client.App.Platforms.Windows;

public partial class App
{
    public App()
    {
        InitializeComponent();
        UnhandledException += OnUnhandledException;
        AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        AppDomain.CurrentDomain.FirstChanceException += CurrentDomain_FirstChanceException;
    }

    private void CurrentDomain_FirstChanceException(object? sender, System.Runtime.ExceptionServices.FirstChanceExceptionEventArgs e)
    {
        System.Diagnostics.Debug.WriteLine($"FirstChanceException: {e.Exception}");
    }

    private void CurrentDomain_UnhandledException(object sender, System.UnhandledExceptionEventArgs e)
    {
        System.Diagnostics.Debug.WriteLine($"UnhandledException: {e.ExceptionObject}");
    }

    private void OnUnhandledException(object sender, Microsoft.UI.Xaml.UnhandledExceptionEventArgs e)
    {
        System.Diagnostics.Debug.WriteLine($"OnUnhandledException: {e.Exception}");
        e.Handled = true;
    }

    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiAppBuilder().Build();
}
