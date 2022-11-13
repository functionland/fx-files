namespace Functionland.FxFiles.Client.Shared.Services.Contracts;

public interface IFileLauncher
{
    public Task OpenWithAsync(string filePath);
}
