namespace Functionland.FxFiles.Client.Shared.Services.Implementations
{
    public class FakePoolService : IFulaPoolSevice
    {
        private readonly List<BloxPool> _BloxPools;
        private readonly List<BloxPool> _AllFulaBloxPools;
        public TimeSpan? ActionLatency { get; set; }
        public TimeSpan? EnumerationLatency { get; set; }
        public IStringLocalizer<AppStrings> StringLocalizer { get; set; } = default!;

        public FakePoolService(IEnumerable<BloxPool> bloxPools, IEnumerable<BloxPool>? allFulaBloxPools = null, TimeSpan? actionLatency = null, TimeSpan? enumerationLatency = null)
        {
            _BloxPools = bloxPools.ToList();
            _AllFulaBloxPools = allFulaBloxPools?.ToList() ?? new List<BloxPool>();

            ActionLatency = actionLatency ?? TimeSpan.FromSeconds(2);
            EnumerationLatency = enumerationLatency ?? TimeSpan.FromMilliseconds(10);
        }
      
        public async Task<List<BloxPool>> GetMyPoolsAsync(CancellationToken? cancellationToken = null)
        {
            if (ActionLatency != null)
            {
                await Task.Delay(ActionLatency.Value);
            }

            return _BloxPools.ToList();
        }
        public async Task LeavePoolAsync(string poolId, CancellationToken? cancellationToken = null)
        {
            if (ActionLatency != null)
            {
                await Task.Delay(ActionLatency.Value);
            }

            var bloxPool = _BloxPools.FirstOrDefault(a => a.Id.ToString() == poolId);

            if (bloxPool is null)
                throw new BloxPoolIsNotFoundException(StringLocalizer.GetString(AppStrings.StreamFileIsNull));

            _BloxPools.Remove(bloxPool);
        }
        public async Task<BloxPoolPurchaseInfo> GetPoolPurchaseInfoAsync(string poolId, CancellationToken? cancellationToken = null)
        {
            if (ActionLatency != null)
            {
                await Task.Delay(ActionLatency.Value);
            }

            return new BloxPoolPurchaseInfo()
            {
                PerMounthPaymentRequired = 150,
                DueNowPaymentRequired = 5
            };
        }
        public async Task<bool> JoinToPoolAsync(string poolId, CancellationToken? cancellationToken = null)
        {
            if (ActionLatency != null)
            {
                await Task.Delay(ActionLatency.Value);
            }

            var bloxPool = _BloxPools.FirstOrDefault(a => a.Id.ToString() == poolId);

            if (bloxPool is null) return false;

            _BloxPools.Add(bloxPool);

            return true;
        }
        public async IAsyncEnumerable<BloxPool> SearchPoolAsync(PoolSearchType filter, double? distance, CancellationToken? cancellationToken = null)
        {
            if (ActionLatency != null)
            {
                await Task.Delay(ActionLatency.Value);
            }

            var bloxPools = new List<BloxPool>();

            if (filter == PoolSearchType.InMyCity)
            {
                for (int i = 0; i <= 2; i++)
                {
                    bloxPools.Add(_AllFulaBloxPools[i]);
                }
            }
            else if (filter == PoolSearchType.InMyState)
            {
                for (int i = 0; i <= 6; i++)
                {
                    bloxPools.Add(_AllFulaBloxPools[i]);
                }
            }
            else if (filter == PoolSearchType.WithinDistance || distance >= 50)
            {
                bloxPools = _AllFulaBloxPools;
            }

            foreach (var bloxPool in bloxPools)
            {
                yield return bloxPool;
            }
        }
    }
}
