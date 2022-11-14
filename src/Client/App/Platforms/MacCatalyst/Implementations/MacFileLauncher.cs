using Functionland.FxFiles.Client.App.Implementations;

namespace Functionland.FxFiles.Client.App.Platforms.MacCatalyst.Implementations;

public class MacFileLauncher : LocalFileLauncher
{
    public override async Task OpenWithAsync(string filePath)
    {
        var uri = new Uri($"file://{filePath}");
        await Launcher.OpenAsync(uri);
    }
}
