namespace Functionland.FxFiles.Client.Shared.Services.Contracts;

public interface IBloxService
{
    Task<List<Blox>> GetBloxesAsync(CancellationToken? cancellationToken = null);
    Task<List<Blox>> GetBloxesJoinInvitationAsync(CancellationToken? cancellationToken = null);
    Task AcceptBloxInvitationAsync(Blox blox,CancellationToken? cancellationToken = null);
    Task RejectBloxInvitationAsync(Blox blox, CancellationToken? cancellationToken = null);
    Task ClearBloxDataAsync(Blox blox, CancellationToken? cancellationToken = null);

    //TODO: not clear these 2 methods.
    Task<List<(string? Did, string? Name)>> GetUsersForShare();
    Task<Stream> GetAvatarAsync();
}
