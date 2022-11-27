namespace Functionland.FxFiles.Client.Shared.Services.Implementations;

public class GoBackService : IGoBackService
{
    public bool CanGoBack { get; set; } = true;
    public bool CanExitApp { get; set; } = false;
    public Func<Task>? GoBackAsync { get;  set; }

    private Func<Task>? _previousGoBackFunc;
    private bool _previousCanGoBack;
    private bool _previousCanExitApp;

    public void SetState(Func<Task>? goBackAsynFunc, bool canGoBack, bool canExitApp)
    {
        SavePreviousState();

        CanExitApp = canExitApp;
        CanGoBack = canGoBack;
        GoBackAsync = goBackAsynFunc;
    }

    public void ResetToPreviousState()
    {
        SetState(_previousGoBackFunc, _previousCanGoBack, _previousCanExitApp);
    }

    private void SavePreviousState()
    {
        _previousGoBackFunc = GoBackAsync;
        _previousCanGoBack =  CanGoBack;
        _previousCanExitApp = CanExitApp;
    }
}