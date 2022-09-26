namespace Functionland.FxFiles.Client.Shared.Services.Contracts;

public interface IZoneService
{
    Task<List<FsZone>> GetZonesAsync(string? searchText = null, CancellationToken? cancellationToken = null);
    Task<IAsyncEnumerable<FsArtifact>> GetZoneArtifactsAsync(int zoneId, FsCategoryFilterType[]? categoryFilters = null, FsTimeFilterType? timeFilter = null, CancellationToken? cancellationToken = null);
    Task<FsZone> CreateZoneAsync(string zoneName, CancellationToken? cancellationToken = null);
    Task AddToZoneAsync(int zoneId, IEnumerable<FsArtifact> artifacts, CancellationToken? cancellationToken = null);
    Task MergeAsync(int sourceZoneId, int destinationZoneId, CancellationToken? cancellationToken = null);
    Task RenameAsync(int zoneId, string newName, CancellationToken? cancellationToken = null);
    Task DeleteAsync(int zoneId, CancellationToken? cancellationToken = null);
    Task ShareZoneAsync(string ZoneName, string dId, CancellationToken? cancellationToken = null);// share my zone with others //TODO: ??
    Task UnShareArtifactAsync(string filePath, string dId, CancellationToken? cancellationToken = null);
}
