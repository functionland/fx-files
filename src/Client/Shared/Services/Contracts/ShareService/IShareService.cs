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
    Task EnsureInitializedAsync();

    /// <summary>
    /// Share an artifact with others
    /// </summary>
    /// <param name="dids"></param>
    /// <param name="fsArtifact"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task ShareArtifactAsync(IEnumerable<string> dids, FsArtifact artifact, CancellationToken? cancellationToken = null);

    /// <summary>
    /// Share some artifacts with others
    /// </summary>
    /// <param name="dids"></param>
    /// <param name="fsArtifact"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task ShareArtifactsAsync(IEnumerable<string> dids, IEnumerable<FsArtifact> fsArtifact, CancellationToken? cancellationToken = null);

    /// <summary>
    ///  Unshare an artifact with others
    /// </summary>
    /// <param name="dids"></param>
    /// <param name="artifactFullPath"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task RevokeShareArtifactAsync(IEnumerable<string> dids, string artifactFullPath, CancellationToken? cancellationToken = null);

    /// <summary>
    /// Unshare some artifacts with others
    /// </summary>
    /// <param name="dids"></param>
    /// <param name="fsArtifact"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task RevokeShareArtifactsAsync(IEnumerable<string> dids, IEnumerable<string> artifactFullPaths, CancellationToken? cancellationToken = null);

    /// <summary>
    /// Get shared artifacts
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    IAsyncEnumerable<FsArtifact> GetSharedByMeArtifactsAsync(CancellationToken? cancellationToken = null);

    IAsyncEnumerable<FsArtifact> GetSharedWithMeArtifactsAsync(CancellationToken? cancellationToken = null);

    Task<bool> IsSahredByMeAsync(string path, CancellationToken? cancellationToken = null);

    /// <summary>
    /// Who has access to an artifact?
    /// </summary>
    /// <param name="artifactFullPath"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<List<FulaUser>> GetArtifactSharesAsync(string path, CancellationToken? cancellationToken = null);
}
