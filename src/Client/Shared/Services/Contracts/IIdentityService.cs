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
    /// Get current user identity
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<FulaUser> GetCurrentUserIdentityAsync(CancellationToken? cancellationToken = null);
}
