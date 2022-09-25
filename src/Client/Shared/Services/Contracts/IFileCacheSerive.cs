namespace Functionland.FxFiles.Client.Shared.Services.Contracts
{
    public interface IFileCacheSerive
    {
        Task MakeFolderAvailableOfflineAsync(string FolderPath);
        Task MakeFileAvailableOfflineAsync(string FilePath);
    }
}
