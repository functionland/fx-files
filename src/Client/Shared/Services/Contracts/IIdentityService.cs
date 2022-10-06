namespace Functionland.FxFiles.Client.Shared.Services.Contracts;

public interface IIdentityService
{
    /// <summary>
    /// Authorize user
    /// </summary>
    /// <param name="dIdDocument"></param>
    /// <param name="securityKey"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<List<FulaUser>> LoginAsync(DIdDocument dIdDocument, string securityKey, CancellationToken? cancellationToken = null);

    void SetCurrentUser(FulaUser user);
    /// <summary>
    /// Get current user identity. Befor call this method, you must login.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<FulaUser> GetCurrentUserAsync(CancellationToken? cancellationToken = null);

    Task<bool> IsLoggedInAsync(CancellationToken? cancellationToken = null);


    // Todo: What is it?

    /// <summary>
    /// Connect to wallet
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <param name="uri"></param>
    /// <returns></returns>
    Task<DIdDocument> ConnectToWalletAsync(Uri uri, CancellationToken? cancellationToken = null);


    /// <summary>
    /// Get other user by did
    /// </summary>
    /// <param name="dId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<FulaUser> GetUserAsync(string dId, CancellationToken? cancellationToken = null);

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
