namespace Functionland.FxFiles.Client.Shared.Services.Implementations
{
    public class InMemoryAppStateStore
    {
        public ViewModeEnum ViewMode { get; private set; } = ViewModeEnum.List;
        public bool IsAvailableForTest { get; private set; }= false;

        public void SetViewMode(ViewModeEnum viewMode)
        {
            ViewMode = viewMode;
        }

        public void SetAvailableForTest( bool isAvailable)
        {
            IsAvailableForTest = isAvailable;
        }
    }
}