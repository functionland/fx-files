namespace Functionland.FxFiles.Client.Shared.Services.Implementations
{
    public partial class FakePoolServiceFactory
    {
        public FakePoolService CreateSimpleBloxPool(TimeSpan? actionLatency = null, TimeSpan? enumerationLatency = null)
        {
            var dict = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("Location", "8.2 miles"),
                new KeyValuePair<string, string>("Join Date", "2022/10/12"),
                new KeyValuePair<string, string>("Count", "38856 devices"),
                new KeyValuePair<string, string>("L2 type", "8.2 Plkadot"),
                new KeyValuePair<string, string>("Replication factor", "x10"),
                new KeyValuePair<string, string>("Disk space", "100 GB"),
                new KeyValuePair<string, string>("Ping speed", "0.08 sec")
            };

            var bloxPool = new FakePoolService(
            new List<BloxPool>
            {
                CreateBloxPool(123457, "The long number of characters in the bloxPool name should be handled ", PoolType.Primary, 
                               DateTimeOffset.Now, 1500, 28000000000, 8000000000, 10000000000, 4000000000, 3000000000, 3000000000, dict)
            },
            actionLatency,
            enumerationLatency);

            return bloxPool;
        }
        public FakePoolService CreateBloxPools(TimeSpan? actionLatency = null, TimeSpan? enumerationLatency = null)
        {
            var now = DateTimeOffset.Now;
            var dict = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("Location", "9.3 miles"),
                new KeyValuePair<string, string>("Join Date", "2022/09/11"),
                new KeyValuePair<string, string>("Count", "300 devices"),
                new KeyValuePair<string, string>("L2 type", "3.2 Plkadot"),
                new KeyValuePair<string, string>("Replication factor", "x11"),
                new KeyValuePair<string, string>("Disk space", "110 GB"),
                new KeyValuePair<string, string>("Ping speed", "0.05 sec")
            };
            var bloxPools = new FakePoolService(
            new List<BloxPool>
            {
                CreateBloxPool(111 ,"BloxPool 1"),
                CreateBloxPool(112 ,"BloxPool 2" , PoolType.Primary),
                CreateBloxPool(113 ,"BloxPool 3" , PoolType.Secondary, now, 1450, 2000000000, 1000000000, 0, 0, 1000000000, 0),
                CreateBloxPool(114 ,"BloxPool 4" , PoolType.Secondary, null, 130, 28000000000, 8000000000, 10000000000, 4000000000, 3000000000, 3000000000,dict),

            },
            actionLatency,
            enumerationLatency);

            return bloxPools;
        }

        public FakePoolService CreateAllFulaBloxPool(TimeSpan? actionLatency = null, TimeSpan? enumerationLatency = null)
        {
            var now = DateTimeOffset.Now;

            var bloxPools = new FakePoolService(
            new List<BloxPool>
            {
                CreateBloxPool(221 ,"BloxPool 2369875523"),
                CreateBloxPool(222 ,"BloxPool 222"),
                CreateBloxPool(223 ,"BloxPool 223" , null, null, null, null, 1000000000, 0, 0, 1000000000, 0),
                CreateBloxPool(224 ,"BloxPool 224" , null, null, 100, null, 8000000000, 2000000000, 4000000000, 3000000000, 3000000000),
                CreateBloxPool(225 ,"BloxPool 225" , null, null, 200, null, 0, 0, 0, 0, 0),
                CreateBloxPool(226 ,"BloxPool 226" , null, null, null, null, 1000000000, 2000000000, 0, 0, 0),
                CreateBloxPool(227 ,"BloxPool 227" , null, now, 400, 9000000000, 5000000000, 2000000000, 0, 3000000000, 0),
                CreateBloxPool(228 ,"BloxPool 228" , null, now, 13020000, 10000000000, 5000000000, 2000000000, 0, 3000000000, 0),
                CreateBloxPool(2291258 ,"The long number of characters in the bloxPool name should be handled " , null, now, 1542, 17000000000, 5000000000, 2000000000, 2000000000, 3000000000,5000000000),
                CreateBloxPool(1235 ,"BloxPool 78975" , null, now, 1400, 20000000000, 5000000000, 2000000000, 5000000000, 3000000000, 5000000000),
                CreateBloxPool(8965 ,"BloxPool 2587428" , null, now, 1560, 11000000000, 5000000000, 2000000000, 0, 3000000000, 0),
                CreateBloxPool(87965 ,"The long number of characters in the bloxPool name " , null, now, 1500, 14000000000, 2000000000, 2000000000, 1000000000, 4000000000,5000000000),
                CreateBloxPool(1365 ,"BloxPool 875461" , null, now, 1600, 181000000000, 1000000000, 80000000000, 40000000000, 30000000000, 30000000000),
                CreateBloxPool(336 ,"BloxPool 16" , null, now, 1000, 2000000000, 1000000000, 0, 0, 1000000000, 0),
                CreateBloxPool(12 ,"BloxPool 17" , null, now, 2000, null, 8000000000, 2000000000, 4000000000, 3000000000, 3000000000),
                CreateBloxPool(589 ,"BloxPool 18" , null, now, 1000, null, 0, 0, 0, 0, 0),

            },
            actionLatency,
            enumerationLatency);

            return bloxPools;
        }

        public FakePoolService CreateTypicalBloxPools(TimeSpan? actionLatency = null, TimeSpan? enumerationLatency = null)
        {
            var now = DateTimeOffset.Now;

            var bloxPools = new FakePoolService(
            new List<BloxPool>
            {
                CreateBloxPool(875461 ,"BloxPool 875461" , PoolType.Primary, now, 1546, 121000000000, 1000000000, 20000000000, 40000000000, 30000000000, 30000000000),
            },
            new List<BloxPool>
            {
                CreateBloxPool(78975 ,"BloxPool 78975" , null, now, 156, 20000000000, 5000000000, 2000000000, 5000000000, 3000000000, 5000000000),
                CreateBloxPool(2587428 ,"BloxPool 2587428" , null, now, 12, 10000000000, 5000000000, 2000000000, 0, 3000000000, 0),
                CreateBloxPool(2291258 ,"The long number of characters in the bloxPool name should be handled " , null, now, 150, 14000000000, 2000000000, 2000000000, 1000000000, 4000000000,5000000000)
            },
            actionLatency,
            enumerationLatency);

            return bloxPools;
        }

        public FakePoolService CreateALotOfBloxPools(TimeSpan? actionLatency = null, TimeSpan? enumerationLatency = null)
        {
            var now = DateTimeOffset.Now;
            var dict = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("Location", "100 miles"),
                new KeyValuePair<string, string>("Join Date", "2022/10/01"),
                new KeyValuePair<string, string>("Count", "45 devices"),
                new KeyValuePair<string, string>("L2 type", "3.2 Plkadot"),
                new KeyValuePair<string, string>("Replication factor", "x5"),
                new KeyValuePair<string, string>("Disk space", "500 GB"),
                new KeyValuePair<string, string>("Ping speed", "0.08 sec")
            };
            var bloxPools = new FakePoolService(
            new List<BloxPool>
            {
                CreateBloxPool(1111 ,"BloxPool One" , PoolType.Primary, now, 1546, 121000000000, 1000000000, 20000000000, 40000000000, 30000000000, 30000000000,dict),
                CreateBloxPool(1112 ,"BloxPool Two", PoolType.Secondary),
                CreateBloxPool(1113 ,"BloxPool Three", PoolType.Secondary),
                CreateBloxPool(1114 ,"BloxPool Four" , PoolType.Secondary, now, 1300, 5000000000, 1000000000,1000000000, 1000000000, 1000000000, 1000000000),
                CreateBloxPool(1115,"BloxPool Five" , PoolType.Secondary, now, 87650, null, 8000000000, 2000000000, 4000000000, 3000000000, 3000000000),
                CreateBloxPool(1116 ,"BloxPool Six" , PoolType.Secondary, null, 2100, 0, 0, 0, 0, 0, 0),
                CreateBloxPool(1117 ,"BloxPool Seven" , PoolType.Secondary, null, null, 3000000000, 1000000000, 2000000000, 0, 0, 0),
                CreateBloxPool(1118 ,"BloxPool Eight" , PoolType.Secondary, now, 400, 9000000000, 5000000000, 2000000000, 0, 3000000000, 0),
                CreateBloxPool(1119 ,"BloxPool Nine" , PoolType.Secondary, now, 13020000, 10000000000, 5000000000, 2000000000, 0, 3000000000, 0),
                CreateBloxPool(25875 ,"The long number of characters in the bloxPool name should be handled 1235" , PoolType.Secondary, now, 875200, 13000000000, 1000000000, 2000000000, 2000000000, 3000000000,5000000000,dict)
            },
            new List<BloxPool>
            {
                CreateBloxPool(1 ,"BloxPool 78975" , null, now, 1400, 20000000000, 5000000000, 2000000000, 5000000000, 3000000000, 5000000000),
                CreateBloxPool(11 ,"BloxPool 2587428" , null, now, 1560, 11000000000, 5000000000, 2000000000, 0, 3000000000, 0),
                CreateBloxPool(12 ,"The long number of characters in the bloxPool name " , null, now, 1500, 14000000000, 2000000000, 2000000000, 1000000000, 4000000000,5000000000),
                CreateBloxPool(13 ,"BloxPool 875461" , null, now, 1600, 181000000000, 1000000000, 80000000000, 40000000000, 30000000000, 30000000000),
                CreateBloxPool(14 ,"BloxPool 14"),
                CreateBloxPool(15 ,"BloxPool 15"),
                CreateBloxPool(16 ,"BloxPool 16" , null, null, 18000, 2000000000, 1000000000, 0, 0, 1000000000, 0),
                CreateBloxPool(17 ,"BloxPool 17" , null, null, 1200, null, 8000000000, 2000000000, 4000000000, 3000000000, 3000000000),
                CreateBloxPool(18 ,"BloxPool 18" , null, null, 2100, null, 0, 0, 0, 0, 0),
                CreateBloxPool(19 ,"BloxPool 19" , null, null, 25100, 3000000000, 1000000000, 2000000000, 0, 0, 0),
                CreateBloxPool(20 ,"BloxPool 20" , null, now, 400, 9000000000, 5000000000, 2000000000, 0, 3000000000, 0),
                CreateBloxPool(21 ,"BloxPool 21" , null, now, 13020000),
                CreateBloxPool(22 ,"The long number of characters in the bloxPool name should be handled " , null, now, 89500, 17000000000, 5000000000, 2000000000, 2000000000, 3000000000,5000000000)
            },
            actionLatency,
            enumerationLatency);

            return bloxPools;
        }
        private static BloxPool CreateBloxPool(int? poolId,
                                       string name,
                                       PoolType? poolType = null,
                                       DateTimeOffset? lastUpdate = null,
                                       decimal? monthlyRate = null,
                                       decimal? currentUse = null,
                                       long? photosUsed = null,
                                       long? videosUsed = null,
                                       long? audiosUsed = null,
                                       long? docsUsed = null,
                                       long? otherUsed = null,
                                       List<KeyValuePair<string, string>>? additinalInformation = null)
        {
            var bloxPool = new BloxPool()
            {
                Id = poolId,
                Name = name,
                PoolType = poolType,
                LastUpdate = lastUpdate,
                MonthlyRate = monthlyRate,
                CurrentUse = currentUse,
                PhotosUsed = photosUsed,
                VideosUsed = videosUsed,
                AudiosUsed = audiosUsed,
                DocsUsed = docsUsed,
                OtherUsed = otherUsed,
                AdditinalInformation = additinalInformation
            };

            return bloxPool;
        }
    }
}
