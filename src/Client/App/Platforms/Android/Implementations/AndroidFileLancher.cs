using Functionland.FxFiles.Client.App.Implementations;
using Functionland.FxFiles.Client.Shared.Models;

namespace Functionland.FxFiles.Client.App.Platforms.Android.Implementations;

public class AndroidFileLauncher : LocalFileLauncher
{
    public override async Task OpenFileAsync(string filePath)
    {
        await Launcher.OpenAsync(new OpenFileRequest
        {
            File = new ReadOnlyFile(filePath)
        });
    }
}
