namespace Functionland.FxFiles.Client.App.Implementations
{
    public interface INativeNavigation
    {
        Task NavigateToVidoeViewer(string path, EventCallback onBack);
    }
}