namespace Functionland.FxFiles.Client.App.Implementations;

public abstract class LocalFileLauncher : IFileLauncher
{
    public virtual async Task OpenWithAsync(string filePath)
    {
        await Launcher.OpenAsync(new OpenFileRequest
        {
            Title = "Open with",
            File = new ReadOnlyFile(filePath)
        });
    }
}
