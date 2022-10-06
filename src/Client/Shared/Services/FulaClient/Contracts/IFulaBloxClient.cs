namespace Functionland.FxFiles.Client.Shared.Services.FulaClient.Contracts;

public interface IFulaBloxClient
{
    Task<List<Blox>> GetBloxesAsync(string token, CancellationToken? cancellationToken = null);
    Task<Blox> FillBloxStatsAsync(string token, string bloxId, CancellationToken? cancellationToken = null);
    Task<List<Blox>> GetBloxInvitationsAsync(string token, CancellationToken? cancellationToken = null);
    Task AcceptBloxInvitationAsync(string token, string bloxId, CancellationToken? cancellationToken = null);
    Task RejectBloxInvitationAsync(string token, string bloxId, CancellationToken? cancellationToken = null);
    Task ClearBloxDataAsync(string token, string bloxId, CancellationToken? cancellationToken = null);

    Task<List<BloxPool>> GetMyPoolsAsync(string token, CancellationToken? cancellationToken = null);
    Task<bool> JoinToPoolAsync(string token, string poolId, CancellationToken? cancellationToken = null);
    Task LeavePoolAsync(string token, string poolId, CancellationToken? cancellationToken = null);
    Task<BloxPoolPurchaseInfo> GetPoolPurchaseInfoAsync(string token, string poolId, CancellationToken? cancellationToken = null);
    IAsyncEnumerable<BloxPool> SearchPoolAsync(string token, PoolSearchType filter, double? distance, CancellationToken? cancellationToken = null);
}
