namespace Functionland.FxFiles.Client.Shared.Services.Contracts;

public interface IIdentityService
{
    /// <summary>
    /// Authorize user
    /// </summary>
    /// <param name="did"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<List<FulaUser>> LoginAsync(string did, CancellationToken? cancellationToken = null);

    /// <summary>
    /// Change user account
    /// </summary>
    /// <param name="did"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<FulaUser> ChangeTokenAsync(string did, CancellationToken? cancellationToken = null);

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
    Task<FulaUser> GetUserAsync(string did, CancellationToken? cancellationToken = null);

    /// <summary>
    /// Get other users by did
    /// </summary>
    /// <param name="dids"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<List<FulaUser>> GetUsersAsync(IEnumerable<string> dids, CancellationToken? cancellationToken = null);

    /// <summary>
    /// Get my avatar
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Stream> GetMyAvatarAsync(CancellationToken? cancellationToken = null);

    /// <summary>
    /// Get other user's avatar
    /// </summary>
    /// <param name="did"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Stream> GetAvatarAsync(string did, CancellationToken? cancellationToken = null);

    /// <summary>
    /// Get avatar thumbnail by did
    /// </summary>
    /// <param name="did"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<string> GetAvatarThumbnailUrlAsync(string did, CancellationToken? cancellationToken = null);
}
