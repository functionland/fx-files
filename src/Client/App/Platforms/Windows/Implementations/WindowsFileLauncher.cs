using Functionland.FxFiles.Client.App.Implementations;
using windows = Windows;

namespace Functionland.FxFiles.Client.App.Platforms.Windows.Implementations;

public class WindowsFileLauncher : LocalFileLauncher
{
    public override async Task OpenWithAsync(string filePath)
    {
        var uri = new Uri(filePath);
        windows.System.LauncherOptions options = new()
        {
            DisplayApplicationPicker = true,
        };
        await windows.System.Launcher.LaunchUriAsync(uri, options);
    }
}
