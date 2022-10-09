using System;
namespace Functionland.FxFiles.Client.Shared.Services.Contracts;

public class IGoBackService
{
    public bool CanGoBack { get; set; } = true;
    public Func<Task>? GoBackAsync { get; set; }
}
