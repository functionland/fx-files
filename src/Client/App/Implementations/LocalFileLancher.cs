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
}
