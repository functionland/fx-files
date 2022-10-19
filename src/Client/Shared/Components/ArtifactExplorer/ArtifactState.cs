namespace Functionland.FxFiles.Client.Shared.Components
{
    public class ArtifactState
    {
        public ViewModeEnum ViewMode { get; private set; } = ViewModeEnum.List;

        public void SetViewMode(ViewModeEnum viewMode)
        {
            ViewMode = viewMode;
        }
    }
}