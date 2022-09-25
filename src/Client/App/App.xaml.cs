[assembly: XamlCompilation(XamlCompilationOptions.Compile)]

namespace Functionland.FxFiles.Client.App;

public partial class App
{
    private IFxLocalDbService FxLocalDbService { get; }
    private IExceptionHandler ExceptionHandler { get; }
    private IPinService PinService { get; }

    public App(IFxLocalDbService fxLocalDbService, IExceptionHandler exceptionHandler, IPinService pinService)
    {
        InitializeComponent();
        FxLocalDbService = fxLocalDbService;
        ExceptionHandler = exceptionHandler;
        PinService = pinService;
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
            }
            catch (Exception exp)
            {
                ExceptionHandler.Handle(exp);
            }
        });
    }
}
