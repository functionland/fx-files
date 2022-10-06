namespace Functionland.FxFiles.Client.Shared.Services.Contracts;

public interface IOfflineAvailablityService
{
    /// <summary>
    /// Fill inline cache data (availabl offlined data) from database
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task InitAsync(CancellationToken? cancellationToken = null);

    /// <summary>
    /// Ensure that service initilized before all methods
    /// </summary>
    /// <returns></returns>
    Task EnsureInitializedAsync();

    /// <summary>
    /// Make available offline an artifact
    /// </summary>
    /// <param name="artifact"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task MakeAvailableOfflineAsync(FsArtifact artifact, CancellationToken? cancellationToken = null);

    /// <summary>
    /// Remove an artifact from available offlive
    /// </summary>
    /// <param name="artifact"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task RemoveAvailableOfflineAsync(FsArtifact artifact, CancellationToken? cancellationToken = null);

    /// <summary>
    /// Check is available offline an artifact
    /// </summary>
    /// <param name="artifact"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<bool> IsAvailableOfflineAsync(FsArtifact artifact, CancellationToken? cancellationToken = null);

    /// <summary>
    /// Get fula local folder address
    /// </summary>
    /// <returns></returns>
    //string GetFulaLocalFolderAddress();
}
