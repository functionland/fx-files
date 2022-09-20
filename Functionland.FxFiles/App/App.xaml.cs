using Prism.Events;
using System.ComponentModel;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]

namespace Functionland.FxFiles.App;

public partial class App
{
    private IFileWatchService FileWatchService { get; }
    private IFxLocalDbService FxLocalDbService { get; }
    private IExceptionHandler ExceptionHandler { get; }
    private IPinService PinService { get; }

    public App(IFxLocalDbService fxLocalDbService, IExceptionHandler exceptionHandler, IPinService pinService, IFileWatchService fileWatchService)
    {
        InitializeComponent();
        FxLocalDbService = fxLocalDbService;
        ExceptionHandler = exceptionHandler;
        PinService = pinService;
        FileWatchService = fileWatchService;
    }

    protected override void OnStart()
    {
        base.OnStart();
        _ = Task.Run(async () =>
        {
            try
            {
                await FxLocalDbService.InitAsync();
                await PinService.InitializeAsync();
                await FileWatchService.InitialyzeAsync();
            }
            catch (Exception exp)
            {
                ExceptionHandler.Handle(exp);
            }
        });
    }
}
