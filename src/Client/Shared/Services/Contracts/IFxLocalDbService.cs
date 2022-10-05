namespace Functionland.FxFiles.Client.Shared.Services.Contracts;

public interface IFxLocalDbService
{
    Task AddPinAsync(FsArtifact artifact);
    Task<List<PinnedArtifact>> GetPinnedArticatInfos();
    Task InitAsync();
    Task RemovePinAsync(string FullPath);
    Task UpdatePinAsync(PinnedArtifact pinnedArtifact, string? oldPath = null);
}