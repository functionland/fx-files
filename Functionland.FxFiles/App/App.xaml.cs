using Functionland.FxFiles.Shared.TestInfra.Implementations;
using System.ComponentModel;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]

namespace Functionland.FxFiles.App;

public partial class App
{
    [AutoInject] IFxLocalDbService FxLocalDbService { get; set; }
    [AutoInject] IExceptionHandler ExceptionHandler { get; set; }
    public App()
    {
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
