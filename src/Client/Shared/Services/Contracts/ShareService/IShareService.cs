namespace Functionland.FxFiles.Client.Shared.Services.Contracts;

public interface IShareService
{
    /// <summary>
    /// Fill inline cache data (shared artifacts) from database
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task InitAsync(CancellationToken? cancellationToken = null);

    /// <summary>
    /// Ensure that service initilized before all methods
    /// </summary>
    /// <returns></returns>
    Task EnsureInitializedAsync(CancellationToken? cancellationToken = null);

    /// <summary>
    /// Set permission to user for access to an artifact with specific accessibility
    /// </summary>
    /// <param name="permissionInfos"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task SetPermissionArtifactsAsync(IEnumerable<ArtifactPermissionInfo> permissionInfos, CancellationToken? cancellationToken = null);

    /// <summary>
    /// Get shared by me artifacts
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    IAsyncEnumerable<FsArtifact> GetSharedByMeArtifactsAsync(CancellationToken? cancellationToken = null);

    /// <summary>
    /// Get shared by with artifacts
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    IAsyncEnumerable<FsArtifact> GetSharedWithMeArtifactsAsync(CancellationToken? cancellationToken = null);

    /// <summary>
    /// Is shred by me?
    /// </summary>
    /// <param name="path"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<bool> IsSahredByMeAsync(string path, CancellationToken? cancellationToken = null);

    /// <summary>
    /// Who has access to an artifact?
    /// </summary>
    /// <param name="path"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<List<ArtifactPermissionInfo>> GetArtifactSharesAsync(string path, CancellationToken? cancellationToken = null);
}
