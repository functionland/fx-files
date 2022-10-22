using System;
namespace Functionland.FxFiles.Client.Shared.Services.Contracts;

public interface IGoBackService
{
    public bool CanGoBack { get; protected set; }
    public bool CanExitApp { get; protected set; }
    public Func<Task>? GoBackAsync { get; protected set; }
    public void OnInit(Func<Task>? goBackAsynFunc, bool canGoBack, bool canExitApp);
}
