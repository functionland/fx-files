namespace Functionland.FxFiles.Client.Shared.Services.Contracts;

public interface IBloxService
{
    Task LoginAsync(string did);
    Task<List<Blox>> GetBloxesAsync(CancellationToken? cancellationToken = null);
    Task FillBloxStatsAsync(Blox blox, CancellationToken? cancellationToken = null);
    Task<List<Blox>> GetBloxInvitationsAsync(CancellationToken? cancellationToken = null);
    Task AcceptBloxInvitationAsync(string bloxId,CancellationToken? cancellationToken = null);
    Task RejectBloxInvitationAsync(string bloxId, CancellationToken? cancellationToken = null);
    Task ClearBloxDataAsync(string bloxId, CancellationToken? cancellationToken = null);
}
