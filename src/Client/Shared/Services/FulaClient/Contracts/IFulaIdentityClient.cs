namespace Functionland.FxFiles.Client.Shared.Services.FulaClient.Contracts;

public interface IFulaIdentityClient
{
    Task<FulaUser> GetMyUserAsync(string did, CancellationToken? cancellationToken = null);
    Task<FulaUser> GetUserAsync(string myDid, string otherDid, CancellationToken? cancellationToken = null);
    Task<List<FulaUser>> GetUsersAsync(string myDid, IEnumerable<string> otherDids, CancellationToken? cancellationToken = null);
    Task<Stream> GetMyAvatarAsync(string did, CancellationToken? cancellationToken = null);
    Task<Stream> GetAvatarAsync(string did, CancellationToken? cancellationToken = null);
    Task<string> GetThumbnailUrlAsync(string did, CancellationToken? cancellationToken = null);
}
