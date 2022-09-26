namespace Functionland.FxFiles.Client.Shared.Services.Contracts;

public interface IFileCacheSerive
{
    Task InitAsync(CancellationToken? cancellationToken = null);
    Task MakeFolderAvailableOfflineAsync(FsArtifact fsArtifact, CancellationToken? cancellationToken = null);
    Task MakeFileAvailableOfflineAsync(FsArtifact fsArtifact, CancellationToken? cancellationToken = null);
    Task<bool> IsAvailableOfflineAsync(FsArtifact fsArtifact, CancellationToken? cancellationToken = null);
}
