namespace Functionland.FxFiles.Client.App.Implementations;

public abstract class LocalFileLauncher : IFileLauncher
{
    public abstract Task<bool> OpenWithAsync(string filePath);

    public abstract Task OpenFileAsync(string filePath);
}
