namespace Functionland.FxFiles.Client.Shared.Services.Contracts;

public interface IFileLauncher
{
     Task<bool> OpenWithAsync(string filePath);
     Task OpenFileAsync(string filePath);
}
