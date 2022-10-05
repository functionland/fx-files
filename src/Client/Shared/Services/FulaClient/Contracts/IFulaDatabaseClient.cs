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
    Task SetPinAsync(DIdDocument dIdDocument, string[] paths, CancellationToken? cancellationToken = null);

    /// <summary>
    /// Unpin some paths.
    /// </summary>
    /// <param name="did"></param>
    /// <param name="path"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task SetUnPinAsync(DIdDocument dIdDocument, string[] path, CancellationToken? cancellationToken = null);

    /// <summary>
    /// Get all pinned items.
    /// </summary>
    /// <param name="did"></param>
    /// <returns></returns>
    Task<string[]> GetPinnedAsync(DIdDocument dIdDocument, CancellationToken? cancellationToken = null);

    /// <summary>
    /// Check is pinned a path or not.
    /// </summary>
    /// <param name="did"></param>
    /// <param name="psth"></param>
    /// <returns></returns>
    Task<bool> IsPinnedAsync(DIdDocument dIdDocument, string psth, CancellationToken? cancellationToken = null);

}