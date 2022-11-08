namespace Functionland.FxFiles.Client.Shared.Services.Implementations;

public class FakePoolService : IFulaPoolSevice
{
    private readonly List<BloxPool> _BloxPools = new();
    private readonly List<BloxPool> _AllFulaBloxPools = new();
    public TimeSpan? ActionLatency { get; set; }
    public TimeSpan? EnumerationLatency { get; set; }
    public IStringLocalizer<AppStrings> StringLocalizer { get; set; } = default!;

    public FakePoolService(IServiceProvider serviceProvider,
                           IEnumerable<BloxPool>? bloxPools = null,
                           IEnumerable<BloxPool>? allFulaBloxPools = null,
                           TimeSpan? actionLatency = null,
                           TimeSpan? enumerationLatency = null)
    {
        _BloxPools.Clear();
        _AllFulaBloxPools.Clear();
        StringLocalizer = serviceProvider.GetRequiredService<IStringLocalizer<AppStrings>>();
        ActionLatency = actionLatency ?? TimeSpan.FromSeconds(2);
        EnumerationLatency = enumerationLatency ?? TimeSpan.FromMilliseconds(10);

        if (bloxPools is not null)
        {
            foreach (var bloxPool in bloxPools)
            {
                _BloxPools.Add(bloxPool);
            }
        }
        else
        {
            _BloxPools = new List<BloxPool>();
        }

        if (allFulaBloxPools is not null)
        {
            foreach (var bloxPool in allFulaBloxPools)
            {
                _AllFulaBloxPools.Add(bloxPool);
            }
        }
        else
        {
            _AllFulaBloxPools = new List<BloxPool>();
        }
    }
  
    public async Task<List<BloxPool>> GetMyPoolsAsync(CancellationToken? cancellationToken = null)
    {
        await LatencyActionAsync();

        return _BloxPools.ToList();
    }
    public async Task LeavePoolAsync(string poolId, CancellationToken? cancellationToken = null)
    {
        await LatencyActionAsync();

        var bloxPool = _BloxPools.FirstOrDefault(a => a.Id.ToString() == poolId);

        if (bloxPool is null)
            throw new BloxPoolIsNotFoundException(StringLocalizer.GetString(AppStrings.BloxPoolIsNotFoundException));

        _BloxPools.Remove(bloxPool);
    }
    public async Task<BloxPoolPurchaseInfo> GetPoolPurchaseInfoAsync(string poolId, CancellationToken? cancellationToken = null)
    {
        await LatencyActionAsync();

        return new BloxPoolPurchaseInfo()
        {
            PerMounthPaymentRequired = 1250.45,
            DueNowPaymentRequired = 41.681
        };
    }
    public async Task<bool> JoinToPoolAsync(string poolId, CancellationToken? cancellationToken = null)
    {
        await LatencyActionAsync();

        var pool = _BloxPools.FirstOrDefault(a => a.Id.ToString() == poolId);

        if (pool is not null)
            throw new BloxPoolAlreadyExistsException(StringLocalizer.GetString(AppStrings.BloxPoolAlreadyExistsException));

        var bloxPool = _AllFulaBloxPools.FirstOrDefault(a => a.Id.ToString() == poolId);

        if (bloxPool is null)
            throw new BloxPoolIsNotFoundException(StringLocalizer.GetString(AppStrings.BloxPoolIsNotFoundException));

        _BloxPools.Add(bloxPool);

        return true;
    }
    public async IAsyncEnumerable<BloxPool> SearchPoolAsync(CancellationToken? cancellationToken = null)
    {
        await LatencyActionAsync();

        foreach (var bloxPool in _AllFulaBloxPools)
        {
            yield return bloxPool;
        }
    }
    public async Task LatencyActionAsync()
    {
        if (ActionLatency is not null)
            await Task.Delay(ActionLatency.Value);
    }
}
