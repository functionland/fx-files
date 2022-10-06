namespace Functionland.FxFiles.Client.Shared.Services.FulaClient.Contracts;

public interface IFulaBloxClient
{
    Task<List<Blox>> GetBloxesAsync(DIdDocument dIdDocument, CancellationToken? cancellationToken = null);
    Task<Blox> FillBloxStatsAsync(DIdDocument dIdDocument, string bloxId, CancellationToken? cancellationToken = null);
    Task<List<Blox>> GetBloxInvitationsAsync(DIdDocument dIdDocument, CancellationToken? cancellationToken = null);
    Task AcceptBloxInvitationAsync(DIdDocument dIdDocument, string bloxId, CancellationToken? cancellationToken = null);
    Task RejectBloxInvitationAsync(DIdDocument dIdDocument, string bloxId, CancellationToken? cancellationToken = null);
    Task ClearBloxDataAsync(DIdDocument dIdDocument, string bloxId, CancellationToken? cancellationToken = null);

    Task<List<BloxPool>> GetMyPoolsAsync(DIdDocument dIdDocument, CancellationToken? cancellationToken = null);
    Task LeavePoolAsync(DIdDocument dIdDocument, string poolId, CancellationToken? cancellationToken = null);
    Task<BloxPoolPurchaseInfo> GetPoolPurchaseInfoAsync(DIdDocument dIdDocument, string poolId, CancellationToken? cancellationToken = null);
    Task<bool> JoinToPoolAsync(DIdDocument dIdDocument, string poolId, CancellationToken? cancellationToken = null);
    IAsyncEnumerable<BloxPool> SearchPoolAsync(DIdDocument dIdDocument, PoolSearchType filter, double? distance, CancellationToken? cancellationToken = null);
}
