namespace Functionland.FxFiles.Client.Shared.Services.Implementations
{
    public class InMemoryAppStateStore
    {
        public ViewModeEnum ViewMode { get; private set; } = ViewModeEnum.list;

        public void SetViewMode(ViewModeEnum viewMode)
        {
            ViewMode = viewMode;
        }
    }
}