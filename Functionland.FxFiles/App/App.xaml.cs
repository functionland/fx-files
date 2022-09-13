[assembly: XamlCompilation(XamlCompilationOptions.Compile)]

namespace Functionland.FxFiles.App;

public partial class App
{
    private IFxLocalDbService FxLocalDbService { get; }
    private IExceptionHandler ExceptionHandler { get; }
    public App(IFxLocalDbService fxLocalDbService, IExceptionHandler exceptionHandler)
    {
        FxLocalDbService = fxLocalDbService;
        ExceptionHandler = exceptionHandler;
        InitializeComponent();
    }

    protected override void OnStart()
    {
        base.OnStart();
        _ = Task.Run(async () =>
        {
            try
            {
                await FxLocalDbService.InitAsync();
            }
            catch (Exception exp)
            {
                ExceptionHandler.Handle(exp);
            }
        });
    }
}
