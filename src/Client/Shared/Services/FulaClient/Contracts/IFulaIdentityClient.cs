namespace Functionland.FxFiles.Client.Shared.Services.FulaClient.Contracts;

public interface IFulaIdentityClient
{
    Task<FulaUser> GetUserAsync(DIdDocument dIdDocument, string did, CancellationToken? cancellationToken = null);
    Task<List<FulaUser>> GetUsersAsync(DIdDocument dIdDocument, IEnumerable<string> otherDids, CancellationToken? cancellationToken = null);
    Task<Stream> GetAvatarAsync(DIdDocument dIdDocument, string did, CancellationToken? cancellationToken = null);
}
