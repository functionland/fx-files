namespace Functionland.FxFiles.Client.Shared.Services.Contracts;

public interface IUserService
{
    Task LoginAsync(string did, CancellationToken? cancellationToken = null);
    Task<FulaUser> GetMyUserAsync(CancellationToken? cancellationToken = null);
    Task<FulaUser> GetUserAsync(string did, CancellationToken? cancellationToken = null);
    Task<List<FulaUser>> GetUsersAsync(IEnumerable<string> dids, CancellationToken? cancellationToken = null);
    Task<Stream> GetMyAvatarAsync(CancellationToken? cancellationToken = null);
    Task<Stream> GetAvatarAsync(string did, CancellationToken? cancellationToken = null);
    Task<string> GetThumbnailUrlAsync(string did, CancellationToken? cancellationToken = null);
}
