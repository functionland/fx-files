namespace Functionland.FxFiles.Client.Shared.Services.Contracts
{
    public interface IThumbnailService
    {
        Task<string> MakeThumbnailAsync(FsArtifact fsArtifact, CancellationToken? cancellationToken = null);
    }
}
