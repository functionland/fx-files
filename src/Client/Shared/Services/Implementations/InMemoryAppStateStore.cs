namespace Functionland.FxFiles.Client.Shared.Services.Implementations;

public class InMemoryAppStateStore : IAppStateStore
{
    public ViewModeEnum ViewMode { get; set; }
    public bool IsAvailableForTest { get; set; }

    public void SetViewMode(ViewModeEnum viewMode)
    {
        ViewMode = viewMode;
    }

    public void SetAvailableForTest( bool isAvailable)
    {
        IsAvailableForTest = isAvailable;
    }
}