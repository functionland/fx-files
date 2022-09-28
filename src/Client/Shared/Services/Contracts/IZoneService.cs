namespace Functionland.FxFiles.Client.Shared.Services.Contracts;

public interface IZoneService
{
    Task<List<FsZone>> GetZonesAsync(string? searchText = null, CancellationToken? cancellationToken = null);
    Task<IAsyncEnumerable<FsArtifact>> GetZoneArtifactsAsync(FsZone zone, FsCategoryFilterType[]? categoryFilters = null, FsTimeFilterType? timeFilter = null, CancellationToken? cancellationToken = null);
    Task<FsZone> CreateZoneAsync(string zoneName, CancellationToken? cancellationToken = null);
    Task AddToZoneAsync(FsZone zone, List<FsArtifact> artifacts, CancellationToken? cancellationToken = null);
    Task MergeAsync(FsZone sourceZone, FsZone destinationZone, CancellationToken? cancellationToken = null);
    Task RenameAsync(FsZone zone, string newName, CancellationToken? cancellationToken = null);
    Task DeleteAsync(FsZone zone, CancellationToken? cancellationToken = null);
    Task ShareZoneAsync(FsZone zone, string dId, CancellationToken? cancellationToken = null);
    Task RemoveArtifactFromZoneAsync(FsArtifact fsArtifact, FsZone zone, CancellationToken? cancellationToken = null);
}
