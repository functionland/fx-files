using Functionland.FxFiles.Client.Shared.Models;
using Functionland.FxFiles.Client.Shared.Services.Common;
using Microsoft.UI.Xaml;
using Microsoft.Windows.AppLifecycle;
using Prism.Events;
using System.Drawing;
using Windows.ApplicationModel.Activation;
using Windows.UI;
using LaunchActivatedEventArgs = Microsoft.UI.Xaml.LaunchActivatedEventArgs;

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

    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiAppBuilder();

    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        base.OnLaunched(args);

        var goodArgs = AppInstance.GetCurrent().GetActivatedEventArgs();

        switch (goodArgs.Kind)
        {
            case ExtendedActivationKind.File:
                var intentHolder = Current.Services.GetRequiredService<IntentHolder>();
                var data = goodArgs.Data as IFileActivatedEventArgs;
                if (data == null) return;

                var path = data.Files.Select(file => file.Path).FirstOrDefault();
                intentHolder.FileUrl = path;

                var eventAggregator = Current.Services.GetRequiredService<IEventAggregator>();
                eventAggregator.GetEvent<IntentReceiveEvent>().Publish(new IntentReceiveEvent());
                break;
        }
    }
}
