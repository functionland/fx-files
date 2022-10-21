namespace Functionland.FxFiles.Client.Shared.Services.Implementations;

public partial class FakePoolServiceFactory
{
    public FakePoolService CreateSimpleBloxPool(TimeSpan? actionLatency = null, TimeSpan? enumerationLatency = null)
    {
        var allFulaBloxPools = new List<BloxPool>();

        var primaryInfos = new List<KeyValuePair<string, string>>
        {
            new KeyValuePair<string, string>("Location", "5.2 miles"),
            new KeyValuePair<string, string>("Join Date", "2022/10/01"),
            new KeyValuePair<string, string>("Count", "38856 devices"),
            new KeyValuePair<string, string>("Disk space", "100 GB")
        };

        var keyValueGroup = new List<KeyValuePair<string, string>>
        {
            new KeyValuePair<string, string>("Video", "40 GB"),
            new KeyValuePair<string, string>("Audio", "20 GB")
        };

        var keyValueGroups = new List<KeyValuePair<string, KeyValuePair<string, string>>>
        {
            new KeyValuePair<string, KeyValuePair<string, string>>("X", keyValueGroup[0]),
            new KeyValuePair<string, KeyValuePair<string, string>>("Y", keyValueGroup[1])
        };

        var bloxPool = new FakePoolService(
        new List<BloxPool>
        {
            CreateBloxPool("1459851", primaryInfos, keyValueGroups, 500)
        },
        allFulaBloxPools,
        actionLatency,
        enumerationLatency);

        return bloxPool;
    }

    public FakePoolService CreateBloxPools(TimeSpan? actionLatency = null, TimeSpan? enumerationLatency = null)
    {
        var allFulaBloxPools = new List<BloxPool>();

        var primaryInfos = new List<KeyValuePair<string, string>>
        {
            new KeyValuePair<string, string>("Location", "9.3 miles"),
            new KeyValuePair<string, string>("Ping speed", "0.05 sec")
        };
        var keyValueGroup = new List<KeyValuePair<string, string>>
        {
            new KeyValuePair<string, string>("Picture", "10GB"),
            new KeyValuePair<string, string>("Audio", "20GB"),
            new KeyValuePair<string, string>("Video", "50GB")
        };

        var keyValueGroups = new List<KeyValuePair<string, KeyValuePair<string, string>>>
        {
            new KeyValuePair<string, KeyValuePair<string, string>>("X", keyValueGroup[0]),
            new KeyValuePair<string, KeyValuePair<string, string>>("Y", keyValueGroup[1]),
            new KeyValuePair<string, KeyValuePair<string, string>>("The long number of characters Z", keyValueGroup[2])
        };

        var bloxPools = new FakePoolService(
        new List<BloxPool>
        {
            CreateBloxPool("BloxPool 1", primaryInfos,keyValueGroups, 100),
            CreateBloxPool("BloxPool 2" , primaryInfos, null, 120),
            CreateBloxPool("BloxPool 3", primaryInfos, null, 50 ),
            CreateBloxPool("BloxPool 3",null, keyValueGroups,105)
        },
        allFulaBloxPools,
        actionLatency,
        enumerationLatency);

        return bloxPools;
    }

    public FakePoolService CreateAllFulaBloxPool(TimeSpan? actionLatency = null, TimeSpan? enumerationLatency = null)
    {
        var FullabloxPools = new List<BloxPool>();

        var primaryInfos = new List<KeyValuePair<string, string>>
        {
            new KeyValuePair<string, string>("Count", "100 devices"),
            new KeyValuePair<string, string>("L2 type", "4.5 Plkadot"),
            new KeyValuePair<string, string>("Replication factor", "x11"),
            new KeyValuePair<string, string>("Disk space", "1000 GB"),
        };

        var primaryInfos1 = new List<KeyValuePair<string, string>>
        {
            new KeyValuePair<string, string>("Disk space", "300 GB"),
            new KeyValuePair<string, string>("Ping speed", "0.03 sec")
        };

        var keyValueGroup = new List<KeyValuePair<string, string>>
        {
            new KeyValuePair<string, string>("Picture", "100 GB"),
            new KeyValuePair<string, string>("Document", "150 GB")
        };

        var keyValueGroups = new List<KeyValuePair<string, KeyValuePair<string, string>>>
        {
            new KeyValuePair<string, KeyValuePair<string, string>>("The long number of characters X", keyValueGroup[0]),
            new KeyValuePair<string, KeyValuePair<string, string>>("The long number of characters Y", keyValueGroup[1])
        };

        var secondaryInfos1 = new List<KeyValuePair<string, KeyValuePair<string, string>>>
        {
            new KeyValuePair<string, KeyValuePair<string, string>>("Test", keyValueGroup[0])
        };

        var bloxPools = new FakePoolService(
            FullabloxPools,
            new List<BloxPool>
            {
                CreateBloxPool("BloxPool 2369875523", primaryInfos, null, 100),
                CreateBloxPool("BloxPool 222", primaryInfos, null, 102),
                CreateBloxPool("BloxPool 223", primaryInfos, null, 150),
                CreateBloxPool("BloxPool 224", primaryInfos, null, 50),
                CreateBloxPool("BloxPool 225", primaryInfos1),
                CreateBloxPool("BloxPool 226", primaryInfos1),
                CreateBloxPool("BloxPool 227", primaryInfos1),
                CreateBloxPool("BloxPool 228", null, keyValueGroups, 400),
                CreateBloxPool("The long number of characters in the bloxPool Id should be handled", null, keyValueGroups, 450),
                CreateBloxPool("BloxPool 78975", primaryInfos1, keyValueGroups, 200),
                CreateBloxPool("BloxPool 2587428", primaryInfos1, keyValueGroups,120),
                CreateBloxPool("The long number of characters in the bloxPool Id ", primaryInfos, keyValueGroups, 200),
                CreateBloxPool("BloxPool 875461", primaryInfos1, keyValueGroups, 190),
                CreateBloxPool("BloxPool 16", primaryInfos1, keyValueGroups,130),
                CreateBloxPool("BloxPool 17", primaryInfos1, secondaryInfos1,230),
                CreateBloxPool("BloxPool 18", primaryInfos1, secondaryInfos1,330),

            },
            actionLatency,
            enumerationLatency);

        return bloxPools;
    }

    public FakePoolService CreateTypicalBloxPools(TimeSpan? actionLatency = null, TimeSpan? enumerationLatency = null)
    {
        var primaryInfos = new List<KeyValuePair<string, string>>
        {
            new KeyValuePair<string, string>("Disk space", "300 GB"),
            new KeyValuePair<string, string>("Ping speed", "0.01 sec")
        };

        var keyValueGroup = new List<KeyValuePair<string, string>>
        {
            new KeyValuePair<string, string>("Picture", "100 GB"),
            new KeyValuePair<string, string>("Document", "20 GB"),
            new KeyValuePair<string, string>("Others", "150 GB")
        };

        var keyValueGroups = new List<KeyValuePair<string, KeyValuePair<string, string>>>
        {
            new KeyValuePair<string, KeyValuePair<string, string>>("The long number of characters X", keyValueGroup[0]),
            new KeyValuePair<string, KeyValuePair<string, string>>("The long number of characters Y", keyValueGroup[1]),
            new KeyValuePair<string, KeyValuePair<string, string>>("The long number of characters Y", keyValueGroup[3])
        };

        var bloxPools = new FakePoolService(
        new List<BloxPool>
        {
            CreateBloxPool("BloxPool 875461", primaryInfos, keyValueGroups),
        },
        new List<BloxPool>
        {
            CreateBloxPool("BloxPool 78975", primaryInfos, null, 150),
            CreateBloxPool("BloxPool 2587428", primaryInfos,keyValueGroups, 350),
            CreateBloxPool("The long number of characters in the bloxPool name should be handled ", null,keyValueGroups, 100)
        },
        actionLatency,
        enumerationLatency);

        return bloxPools;
    }

    public FakePoolService CreateALotOfBloxPools(TimeSpan? actionLatency = null, TimeSpan? enumerationLatency = null)
    {
        var primaryInfos = new List<KeyValuePair<string, string>>
        {
            new KeyValuePair<string, string>("Location", "55 miles"),
            new KeyValuePair<string, string>("Join Date", "2022/09/01"),
            new KeyValuePair<string, string>("Count", "120 devices"),
            new KeyValuePair<string, string>("L2 type", "3.2 Plkadot"),
            new KeyValuePair<string, string>("Replication factor", "x5"),
            new KeyValuePair<string, string>("Disk space", "1000 GB"),
            new KeyValuePair<string, string>("Ping speed", "0.05 sec")
        };

        var primaryInfos1 = new List<KeyValuePair<string, string>>
        {
            new KeyValuePair<string, string>("L2 type", "4.3 Plkadot"),
            new KeyValuePair<string, string>("Replication factor", "x5")
        };

        var keyValueGroup = new List<KeyValuePair<string, string>>
        {
            new KeyValuePair<string, string>("Picture", "140 GB"),
            new KeyValuePair<string, string>("Document", "130 GB")
        };

        var keyValueGroups = new List<KeyValuePair<string, KeyValuePair<string, string>>>
        {
            new KeyValuePair<string, KeyValuePair<string, string>>("The long number of characters Pictures", keyValueGroup[0]),
            new KeyValuePair<string, KeyValuePair<string, string>>("The long number of characters Documents", keyValueGroup[1])
        };

        var bloxPools = new FakePoolService(
        new List<BloxPool>
        {
            CreateBloxPool("BloxPool One", primaryInfos, null, 200),
            CreateBloxPool("BloxPool Two", primaryInfos, null, 9),
            CreateBloxPool("BloxPool Three", primaryInfos),
            CreateBloxPool("BloxPool Four", primaryInfos1, null, 700),
            CreateBloxPool("BloxPool Five", primaryInfos1, keyValueGroups,400),
            CreateBloxPool("BloxPool Six", primaryInfos1),
            CreateBloxPool("BloxPool Seven", null, keyValueGroups,60),
            CreateBloxPool("BloxPool Eight", null, keyValueGroups),
            CreateBloxPool("BloxPool Nine", primaryInfos, keyValueGroups, 90),
            CreateBloxPool("The long number of characters in the bloxPool Id should be handled 1235", primaryInfos1, keyValueGroups, 50)
        },
        new List<BloxPool>
        {
            CreateBloxPool("BloxPool 78975",primaryInfos, null, 900),
            CreateBloxPool("BloxPool 2587428",primaryInfos),
            CreateBloxPool("The long number of characters in the bloxPool Id ",primaryInfos1, null, 900),
            CreateBloxPool("BloxPool 875461", primaryInfos1),
            CreateBloxPool("BloxPool 14", primaryInfos1),
            CreateBloxPool("BloxPool 15", primaryInfos1),
            CreateBloxPool("BloxPool 16", primaryInfos1, null, 100),
            CreateBloxPool("BloxPool 17", primaryInfos1, keyValueGroups,1000),
            CreateBloxPool("BloxPool 18", null, keyValueGroups,2000),
            CreateBloxPool("BloxPool 19", null, keyValueGroups,200),
            CreateBloxPool("BloxPool 20", null, keyValueGroups,20),
            CreateBloxPool("BloxPool 21", null, null, 600),
            CreateBloxPool("The long number of characters in the bloxPool Id should be handled", primaryInfos1, keyValueGroups, 400)
        },
        actionLatency,
        enumerationLatency);

        return bloxPools;
    }
    private static BloxPool CreateBloxPool(string id,
                                   List<KeyValuePair<string, string>>? primaryInfos = null,
                                   List<KeyValuePair<string, KeyValuePair<string, string>>>? keyValueGroups = null,
                                   int? pingTime = null)

    {
        var bloxPool = new BloxPool(id)
        {
            PrimaryInfos = primaryInfos,
            KeyValueGroups = keyValueGroups,
            PingTime = pingTime
        };

        return bloxPool;
    }
}
