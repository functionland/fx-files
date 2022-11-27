using Functionland.FxFiles.Client.Shared.Models;
using System.Diagnostics;

using Microsoft.Maui.Controls.PlatformConfiguration;

namespace Functionland.FxFiles.Client.App.Implementations;

public abstract class LocalFileLauncher : IFileLauncher
{
    public virtual async Task<bool> OpenWithAsync(string filePath)
    {
        var isOpen = await Launcher.OpenAsync(new OpenFileRequest
        {
            Title = "Open with",
            File = new ReadOnlyFile(filePath)
        });

        return isOpen;
    }

    public abstract Task OpenFileAsync(string filePath);
}
