namespace Functionland.FxFiles.Client.Shared.Services.Contracts;

public interface IFulaIdentityClient
{
    Task<UserToken> LoginAsync(string dId, string securityKey, CancellationToken? cancellationToken = null);
    Task<List<FulaUser>> GetUsersAsync(string token, IEnumerable<string> otherDids, CancellationToken? cancellationToken = null);
    Task<Stream> GetAvatarAsync(string token, string did, CancellationToken? cancellationToken = null);
}
