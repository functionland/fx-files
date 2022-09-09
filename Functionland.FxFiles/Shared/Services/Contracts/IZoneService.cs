namespace Functionland.FxFiles.Shared.Services.Contracts
{
    public interface IZoneService
    {
        Task<List<FsZone>> GetZonesAsync(string? searchText = null, CancellationToken ? cancellationToken = null);
        Task<IAsyncEnumerable<FsArtifact>> GetZoneArtifactsAsync(int zoneId, FsCategoryFilterType[]? categoryFilters = null, FsTimeFilterType? timeFilter = null, CancellationToken? cancellationToken = null);
        Task<FsZone> CreateZoneAsync(string zoneName, CancellationToken? cancellationToken = null);
        Task<FsZone> ShareAsync(IEnumerable<FsArtifact>? artifacts = null, string[]? dIds = null, CancellationToken? cancellationToken = null);
        Task AddArtifactToZoneAsync(int zoneId, string filePath, CancellationToken? cancellationToken = null);
        Task MergeZoneAsync(int sourceZoneId, int destinationZoneId, CancellationToken? cancellationToken = null);
        Task RenameZoneAsync(int zoneId, string newName, CancellationToken? cancellationToken = null);
        Task DeleteZoneAsync(int zoneId, CancellationToken? cancellationToken = null);
        Task ShareZoneAsync(string filePath, string dId, CancellationToken? cancellationToken = null);
        Task UnShareFolderAsync(string filePath, string dId, CancellationToken? cancellationToken = null);
    }
}
