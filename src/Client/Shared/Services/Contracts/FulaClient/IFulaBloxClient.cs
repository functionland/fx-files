namespace Functionland.FxFiles.Client.Shared.Services.Contracts;

public interface IFulaBloxClient
{
    Task<List<Blox>> GetBloxesAsync(string token, CancellationToken? cancellationToken = null);
    Task<List<Blox>> GetBloxInvitationsAsync(string token, CancellationToken? cancellationToken = null);
    Task AcceptBloxInvitationAsync(string token, string bloxId, CancellationToken? cancellationToken = null);
    Task RejectBloxInvitationAsync(string token, string bloxId, CancellationToken? cancellationToken = null);
    Task<List<BloxPool>> GetMyPoolsAsync(string token, CancellationToken? cancellationToken = null);
    Task JoinToPoolAsync(string token, string poolId, PoolRole poolRole = PoolRole.Secondary, CancellationToken? cancellationToken = null);
    Task LeavePoolAsync(string token, string poolId, CancellationToken? cancellationToken = null);
    Task<BloxPoolPurchaseInfo> GetPoolPurchaseInfoAsync(string token, string poolId, CancellationToken? cancellationToken = null);
    IAsyncEnumerable<BloxPool> SearchPoolAsync(string token, CancellationToken? cancellationToken = null);
}
