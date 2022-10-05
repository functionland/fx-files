namespace Functionland.FxFiles.Client.Shared.Services.Contracts;

public interface IBloxPoolSevice
{
    Task<List<BloxPool>> GetMyPoolsAsync(CancellationToken? cancellationToken = null);
    Task LeavePoolAsync(string poolId, CancellationToken? cancellationToken = null);
    Task<BloxPoolPurchaseInfo> GetPoolPurchaseInfoAsync(string poolId, CancellationToken? cancellationToken = null);
    Task<bool> JoinToPoolAsync(string poolId, CancellationToken? cancellationToken = null);
    IAsyncEnumerable<BloxPool> SearchPoolAsync(PoolSearchType filter, double? distance, CancellationToken? cancellationToken = null);
}
