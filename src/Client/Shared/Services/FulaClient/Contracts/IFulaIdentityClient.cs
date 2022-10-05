namespace Functionland.FxFiles.Client.Shared.Services.FulaClient.Contracts;

public interface IFulaIdentityClient
{
    Task<FulaUser> GetMyUserAsync(DIdDocument dIdDocument, CancellationToken? cancellationToken = null);
    Task<FulaUser> GetUserAsync(DIdDocument dIdDocument, string otherDid, CancellationToken? cancellationToken = null);
    Task<List<FulaUser>> GetUsersAsync(DIdDocument dIdDocument, IEnumerable<string> otherDids, CancellationToken? cancellationToken = null);
    Task<Stream> GetMyAvatarAsync(DIdDocument dIdDocument, CancellationToken? cancellationToken = null);
    Task<Stream> GetAvatarAsync(DIdDocument dIdDocument, string did, CancellationToken? cancellationToken = null);
    Task<string> GetMyThumbnailUrlAsync(DIdDocument dIdDocument, CancellationToken? cancellationToken = null);
    Task<string> GetThumbnailUrlAsync(DIdDocument dIdDocument, string did, CancellationToken? cancellationToken = null);
}
