namespace Functionland.FxFiles.Client.Shared.Services.FulaClient.Contracts;

public interface IFulaDatabaseClient
{
    /// <summary>
    /// Pin some paths.
    /// </summary>
    /// <param name="did">Unique user id</param>
    /// <param name="paths"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task SetPinAsync(string did, string[] paths, CancellationToken? cancellationToken = null);

    /// <summary>
    /// Unpin some paths.
    /// </summary>
    /// <param name="did"></param>
    /// <param name="path"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task SetUnPinAsync(string did, string[] path, CancellationToken? cancellationToken = null);

    /// <summary>
    /// Get all pinned items.
    /// </summary>
    /// <param name="did"></param>
    /// <returns></returns>
    Task<string[]> GetPinnedAsync(string did);

    /// <summary>
    /// Check is pinned a path or not.
    /// </summary>
    /// <param name="did"></param>
    /// <param name="psth"></param>
    /// <returns></returns>
    Task<bool> IsPinnedAsync(string did, string psth);

}