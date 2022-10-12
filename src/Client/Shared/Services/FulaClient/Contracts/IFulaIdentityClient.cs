namespace Functionland.FxFiles.Client.Shared.Services.FulaClient.Contracts;

public interface IFulaIdentityClient
{
    Task<FulaUser> GetUserAsync(string token, string did, CancellationToken? cancellationToken = null);
    Task<List<FulaUser>> GetUsersAsync(string token, IEnumerable<string> otherDids, CancellationToken? cancellationToken = null);
    Task<Stream> GetAvatarAsync(string token, string did, CancellationToken? cancellationToken = null);
    Task<List<Stream>> GetAvatarsAsync(string token, IEnumerable<string> did, CancellationToken? cancellationToken = null);
}
