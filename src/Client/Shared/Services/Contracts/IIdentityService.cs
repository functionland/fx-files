namespace Functionland.FxFiles.Client.Shared.Services.Contracts;

public interface IIdentityService
{
    /// <summary>
    /// Authorize user
    /// </summary>
    /// <param name="did"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<List<FulaUser>> LoginAsync(DIdDocument dIdDocument, CancellationToken? cancellationToken = null);

    /// <summary>
    /// Ensure login call befor all methods
    /// </summary>
    /// <returns></returns>
    Task EnsureLoginedAsync();

    /// <summary>
    /// 
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<DIdDocument> ConnectToWalletAsync(CancellationToken? cancellationToken = null);

    /// <summary>
    /// Change user account
    /// </summary>
    /// <param name="did"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<FulaUser> ChangeTokenAsync(DIdDocument dIdDocument, CancellationToken? cancellationToken = null);

    /// <summary>
    /// Get current user identity. Befor call this method, you must login.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<FulaUser> GetCurrentUserIdentityAsync(CancellationToken? cancellationToken = null);

    /// <summary>
    /// Get other user by did
    /// </summary>
    /// <param name="did"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<FulaUser> GetUserAsync(DIdDocument dIdDocument, CancellationToken? cancellationToken = null);

    /// <summary>
    /// Get other users by did
    /// </summary>
    /// <param name="dids"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<List<FulaUser>> GetUsersAsync(DIdDocument dIdDocument, IEnumerable<string> dids, CancellationToken? cancellationToken = null);

    /// <summary>
    /// Get my avatar
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Stream> GetMyAvatarAsync(DIdDocument dIdDocument, CancellationToken? cancellationToken = null);

    /// <summary>
    /// Get other user's avatar
    /// </summary>
    /// <param name="did"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Stream> GetAvatarAsync(DIdDocument dIdDocument, string did, CancellationToken? cancellationToken = null);

    /// <summary>
    /// Get avatar thumbnail by did
    /// </summary>
    /// <param name="did"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<string> GetAvatarThumbnailUrlAsync(DIdDocument dIdDocument, string did, CancellationToken? cancellationToken = null);
}
