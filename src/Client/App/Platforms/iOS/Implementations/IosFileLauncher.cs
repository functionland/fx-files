using Functionland.FxFiles.Client.App.Implementations;

namespace Functionland.FxFiles.Client.App.Platforms.iOS.Implementations;

public class IosFileLauncher : LocalFileLauncher
{
    public override async Task<bool> OpenWithAsync(string filePath)
    {
        var uri = new Uri($"file://{filePath}");
        var isOpen = await Launcher.OpenAsync(uri);
        return isOpen;
    }
}
