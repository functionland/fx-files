namespace Functionland.FxFiles.Client.Shared.Services.FulaClient.Contracts;

public interface IFulaBloxClient
{
    Task<List<Blox>> GetBloxesAsync(string did, CancellationToken? cancellationToken = null);
    Task<Blox> FillBloxStatsAsync(string did, string bloxId, CancellationToken? cancellationToken = null);
    Task<List<Blox>> GetBloxInvitationsAsync(string did, CancellationToken? cancellationToken = null);
    Task AcceptBloxInvitationAsync(string did, string bloxId, CancellationToken? cancellationToken = null);
    Task RejectBloxInvitationAsync(string did, string bloxId, CancellationToken? cancellationToken = null);
    Task ClearBloxDataAsync(string did, string bloxId, CancellationToken? cancellationToken = null);

    Task<List<BloxPool>> GetMyPoolsAsync(string did, CancellationToken? cancellationToken = null);
    Task LeavePoolAsync(string did, string poolId, CancellationToken? cancellationToken = null);
    Task<BloxPoolPurchaseInfo> GetPoolPurchaseInfoAsync(string did, string poolId, CancellationToken? cancellationToken = null);
    Task<bool> JoinToPoolAsync(string did, string poolId, CancellationToken? cancellationToken = null);
    IAsyncEnumerable<BloxPool> SearchPoolAsync(string did, PoolSearchType filter, double? distance, CancellationToken? cancellationToken = null);
}
