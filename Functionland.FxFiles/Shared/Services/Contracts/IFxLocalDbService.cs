namespace Functionland.FxFiles.Shared.Services.Contracts
{
    public interface IFxLocalDbService
    {
        Task AddPinAsync(FsArtifact artifact);
        Task<List<PinnedArtifact>> GetPinnedArticatInfos();
        Task InitAsync();
        Task RemovePinAsync(string FullPath);
    }
}