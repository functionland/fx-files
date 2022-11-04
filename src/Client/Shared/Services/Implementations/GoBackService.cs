namespace Functionland.FxFiles.Client.Shared.Services.Implementations;

public class GoBackService : IGoBackService
{
    public bool CanGoBack { get; set; } = true;
    public bool CanExitApp { get; set; } = false;
    public Func<Task>? GoBackAsync { get;  set; }

    public void OnInit(Func<Task> goBackAsynFunc, bool canGoBack, bool canExitApp)
    {
        CanExitApp = canExitApp;
        CanGoBack = canGoBack;
        GoBackAsync = goBackAsynFunc;
    }
}