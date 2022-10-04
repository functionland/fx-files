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
    Task ShareFsArtifactAsync(IEnumerable<string> dids, FsArtifact fsArtifact, CancellationToken? cancellationToken = null);

    /// <summary>
    /// Share some artifacts with others
    /// </summary>
    /// <param name="dids"></param>
    /// <param name="fsArtifact"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task ShareFsArtifactsAsync(IEnumerable<string> dids, IEnumerable<FsArtifact> fsArtifact, CancellationToken? cancellationToken = null);

    /// <summary>
    ///  Unshare an artifact with others
    /// </summary>
    /// <param name="dids"></param>
    /// <param name="artifactFullPath"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task UnShareFsArtifactAsync(IEnumerable<string> dids, string artifactFullPath, CancellationToken? cancellationToken = null);

    /// <summary>
    /// Unshare some artifacts with others
    /// </summary>
    /// <param name="dids"></param>
    /// <param name="fsArtifact"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task UnShareFsArtifactsAsync(IEnumerable<string> dids, IEnumerable<string> artifactFullPaths, CancellationToken? cancellationToken = null);

    /// <summary>
    /// Get shared artifacts
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    IAsyncEnumerable<FsArtifact> GetSharedFsArtifactsAsync(CancellationToken? cancellationToken = null);

    /// <summary>
    /// Who has access to an artifact?
    /// </summary>
    /// <param name="artifactFullPath"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<List<FulaUser>> WhoHasAccessToArtifact(string artifactFullPath, CancellationToken? cancellationToken = null);
}
