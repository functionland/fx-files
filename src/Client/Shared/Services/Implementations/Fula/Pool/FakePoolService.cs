using static System.Net.Mime.MediaTypeNames;

namespace Functionland.FxFiles.Client.Shared.Services.Implementations
{
    public class FakePoolService : IFulaPoolSevice
    {
        private readonly List<BloxPool> _BloxPools;
        private readonly List<BloxPool> _AllFulaBloxPools;
        public TimeSpan? ActionLatency { get; set; }
        public TimeSpan? EnumerationLatency { get; set; }

        public FakePoolService(IEnumerable<BloxPool> bloxPool, IEnumerable<BloxPool> allFulaBloxPools, TimeSpan? actionLatency = null, TimeSpan? enumerationLatency = null)
        {
            _BloxPools.Clear();
            _AllFulaBloxPools.Clear();

            ActionLatency = actionLatency ?? TimeSpan.FromSeconds(2);
            EnumerationLatency = enumerationLatency ?? TimeSpan.FromMilliseconds(10);

            foreach (var bloxPools in bloxPool)
            {
                _BloxPools.Add(bloxPools);
            }
            foreach (var allFulaBloxPool in allFulaBloxPools)
            {
                _AllFulaBloxPools.Add(allFulaBloxPool);
            }
        }
        public FakePoolService(IEnumerable<BloxPool> bloxPool, TimeSpan? actionLatency = null, TimeSpan? enumerationLatency = null)
        {
            _BloxPools.Clear();
            ActionLatency = actionLatency ?? TimeSpan.FromSeconds(2);
            EnumerationLatency = enumerationLatency ?? TimeSpan.FromMilliseconds(10);

            foreach (var bloxPools in bloxPool)
            {
                _BloxPools.Add(bloxPools);
            }
        }
        public async Task<List<BloxPool>> GetMyPoolsAsync(CancellationToken? cancellationToken = null)
        {
            return _BloxPools.ToList();
        }
        public async Task LeavePoolAsync(string poolId, CancellationToken? cancellationToken = null)
        {
            var bloxPool = _BloxPools.FirstOrDefault(a => a.Id.ToString() == poolId);

            if (bloxPool is null) return;//TODO
            _BloxPools.Remove(bloxPool);
        }
        public async Task<BloxPoolPurchaseInfo> GetPoolPurchaseInfoAsync(string poolId, CancellationToken? cancellationToken = null)
        {
            return new BloxPoolPurchaseInfo()
            {
                PerMounthPaymentRequired = 150,
                DueNowPaymentRequired = 5
            };
        }
        public async Task<bool> JoinToPoolAsync(string poolId, CancellationToken? cancellationToken = null)
        {
            var bloxPool = _BloxPools.FirstOrDefault(a => a.Id.ToString() == poolId);

            if (bloxPool is null) return false;

            _BloxPools.Add(bloxPool);

            return true;
        }
        public async IAsyncEnumerable<BloxPool> SearchPoolAsync(PoolSearchType filter, double? distance, CancellationToken? cancellationToken = null)
        {
            var bloxPools = new List<BloxPool>();

            if(filter == PoolSearchType.InMyCity)
            {
                for (int i = 0; i <= 2; i++)
                {
                    bloxPools.Add(_AllFulaBloxPools[i]);
                }
            }
            else if(filter == PoolSearchType.InMyState)
            {
                for (int i = 0; i <= 6; i++)
                {
                    bloxPools.Add(_AllFulaBloxPools[i]);
                }
            }
            else if(filter == PoolSearchType.WithinDistance || distance >= 50)
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
