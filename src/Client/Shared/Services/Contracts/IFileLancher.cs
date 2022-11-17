namespace Functionland.FxFiles.Client.Shared.Services.Contracts;

public interface IFileLauncher
{
    public Task<bool> OpenWithAsync(string filePath);
}
