namespace Functionland.FxFiles.Client.Shared.Services.Contracts;

public interface IPoolSevice
{
    Task<IEnumerable<Pool>> GetPoolsAsync(CancellationToken? cancellationToken = null);
    Task LeavePoolAsync(Pool pool, CancellationToken? cancellationToken = null);
    Task<(double? DueNowPaymentRequired, double? PerMounthPaymentRequired)> RequestJoinToPoolAsync(Pool pool, CancellationToken? cancellationToken = null);
    Task<bool> ConfirmJoinToPoolAsync(Pool pool, CancellationToken? cancellationToken = null);
    IAsyncEnumerable<Pool> SearchPoolAsync(PoolFilter filter, double? Distance, CancellationToken? cancellationToken = null);
}
