using System.Diagnostics;

using Functionland.FxFiles.Client.App.Implementations;
using windows = Windows;

namespace Functionland.FxFiles.Client.App.Platforms.Windows.Implementations;

public class WindowsFileLauncher : LocalFileLauncher
{
    public override async Task<bool> OpenWithAsync(string filePath)
    {
        var uri = new Uri(filePath);
        windows.System.LauncherOptions options = new()
        {
            DisplayApplicationPicker = true,
        };
        var isOpen = await windows.System.Launcher.LaunchUriAsync(uri, options);
        return isOpen;
    }

    public override async Task OpenFileAsync(string filePath)
    {
        new Process
        {
            StartInfo = new ProcessStartInfo(filePath)
            {
                UseShellExecute = true
            }
        }.Start();
    }
}
