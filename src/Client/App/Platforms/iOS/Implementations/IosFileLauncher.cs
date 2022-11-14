using Functionland.FxFiles.Client.App.Implementations;
using Functionland.FxFiles.Client.Shared.Models;

namespace Functionland.FxFiles.Client.App.Platforms.iOS.Implementations;

public class IosFileLauncher : LocalFileLauncher
{
    public override async Task OpenWithAsync(string filePath)
    {
        var uri = new Uri($"file://{filePath}");
        await Launcher.OpenAsync(uri);
    }
}
