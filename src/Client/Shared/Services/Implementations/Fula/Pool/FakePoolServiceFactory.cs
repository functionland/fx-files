namespace Functionland.FxFiles.Client.Shared.Services.Implementations;

public partial class FakePoolServiceFactory
{
    [AutoInject] public IServiceProvider ServiceProvider { get; set; }

    public FakePoolService CreateSimpleBloxPool(TimeSpan? actionLatency = null, TimeSpan? enumerationLatency = null)
    {
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

        var bloxPool = new FakePoolService(ServiceProvider,
        new List<BloxPool>
        {
            CreateBloxPool("1459851fasjkjsahciaishv1235648", primaryInfos, keyValueGroups, 500)
        },
        new List<BloxPool>
        {
            CreateBloxPool("1459851fasjkjsahciaishv1235648", primaryInfos, keyValueGroups, 500)
        },
        actionLatency,
        enumerationLatency);

        return bloxPool;
    }

    public FakePoolService CreateBloxPools(TimeSpan? actionLatency = null, TimeSpan? enumerationLatency = null)
    {
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

        var bloxPools = new FakePoolService(ServiceProvider,
        new List<BloxPool>
        {
            CreateBloxPool("a4s5d5df5ff5f5123665494235525h", primaryInfos,keyValueGroups, 100),
            CreateBloxPool("qazzdjdiiowppppppppjjjj4452397" , primaryInfos, null, 120),
            CreateBloxPool("qqqqqqtttttuuuudbddinisoansiai", primaryInfos, null, 50 ),
            CreateBloxPool("85741656325655sbhsjsabsdvashdv",null, keyValueGroups,105)
        },
        new List<BloxPool>
        {
            CreateBloxPool("a4s5d5df5ff5f5123665494235525h", primaryInfos,keyValueGroups, 100),
            CreateBloxPool("qazzdjdiiowppppppppjjjj4452397" , primaryInfos, null, 120),
            CreateBloxPool("qqqqqqtttttuuuudbddinisoansiai", primaryInfos, null, 50 ),
            CreateBloxPool("85741656325655sbhsjsabsdvashdv",null, keyValueGroups,105)
        },
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

        var bloxPools = new FakePoolService(ServiceProvider,
            FullabloxPools,
            new List<BloxPool>
            {
                CreateBloxPool("123654kklljjhhujkoiu123698cvbn", primaryInfos, null, 100),
                CreateBloxPool("AAAssserdh123666qwwerrttyyuu77", primaryInfos, null, 102),
                CreateBloxPool("14785236987456321jiunbvhjjbhhj", primaryInfos, null, 150),
                CreateBloxPool("BloxPool2241111111111111111111", primaryInfos, null, 50),
                CreateBloxPool("BloxPool2258799999999999666666", primaryInfos1),
                CreateBloxPool("BloxPool226zaqwssxvcdertgvhhnj", primaryInfos1),
                CreateBloxPool("BloxPool2271478525468646546548", primaryInfos1),
                CreateBloxPool("8744166325545454jjjghffgcfxgfc", null, keyValueGroups, 400),
                CreateBloxPool("The long number of characters in the bloxPool Id should be handled", null, keyValueGroups, 450),
                CreateBloxPool("BloxPool78975teteuwiiddwwdhwkp", primaryInfos1, keyValueGroups, 200),
                CreateBloxPool("BloxPoolbxdhjdidiiijdjf2587428", primaryInfos1, keyValueGroups,120),
                CreateBloxPool("BloxPool875461hhhggfdsdfgkjjjj", primaryInfos1, keyValueGroups, 190)

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
            new KeyValuePair<string, KeyValuePair<string, string>>("The long number of characters Y", keyValueGroup[2])
        };

        var bloxPools = new FakePoolService(ServiceProvider,
        new List<BloxPool>
        {
            CreateBloxPool("BloxPool8754611478965332655557", primaryInfos, keyValueGroups),
        },
        new List<BloxPool>
        {
            CreateBloxPool("BloxPool8754611478965332655557", primaryInfos, keyValueGroups),
            CreateBloxPool("zvgssdsdbjAAAEEEdjddkdk123654s", primaryInfos, null, 150),
            CreateBloxPool("BloxPool2587428zxdarasbjdjsdbd", primaryInfos,keyValueGroups, 350),
            CreateBloxPool("The long number of characters in the bloxPool name should be handled", null,keyValueGroups, 100)
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

        var bloxPools = new FakePoolService(ServiceProvider,
        new List<BloxPool>
        {
            CreateBloxPool("BloxPoolOne8754611478965332655", primaryInfos, null, 200),
            CreateBloxPool("BloxPoolTwo1239966887412589632", primaryInfos, null, 9),
            CreateBloxPool("BloxPoolThree123456789nnnnnnmm", primaryInfos),
            CreateBloxPool("BloxPoolFour149043846952554715", primaryInfos1, null, 700),
            CreateBloxPool("BloxPoolFivezsweaysujbjisonaoo", primaryInfos1, keyValueGroups,400),
            CreateBloxPool("BloxPoolSixqwertyuiopljjgghhjj", primaryInfos1),
            CreateBloxPool("BloxPool147852369987uyttrrNine", primaryInfos, keyValueGroups, 90),
            CreateBloxPool("The long number of characters in the bloxPool Id should be handled 1235", primaryInfos1, keyValueGroups, 50)
        },
        new List<BloxPool>
        {
            CreateBloxPool("BloxPoolOne8754611478965332655", primaryInfos, null, 200),
            CreateBloxPool("BloxPoolTwo1239966887412589632", primaryInfos, null, 9),
            CreateBloxPool("BloxPoolThree123456789nnnnnnmm", primaryInfos),
            CreateBloxPool("BloxPoolFour149043846952554715", primaryInfos1, null, 700),
            CreateBloxPool("BloxPoolFivezsweaysujbjisonaoo", primaryInfos1, keyValueGroups,400),
            CreateBloxPool("BloxPoolSixqwertyuiopljjgghhjj", primaryInfos1),
            CreateBloxPool("BloxPool147852369987uyttrrNine", primaryInfos, keyValueGroups, 90),
            CreateBloxPool("The long number of characters in the bloxPool Id should be handled 1235", primaryInfos1, keyValueGroups, 50),
            CreateBloxPool("BloxPoolOneIdbloxId12378965412", primaryInfos, null, 200),
            CreateBloxPool("zvgssdsdbjAAAEEEdjddkdk123654s",primaryInfos, null, 900),
            CreateBloxPool("BloxPool2587428zxdarasbjdjsdbd",primaryInfos),
            CreateBloxPool("BloxPool875461BloxPoolOne87546", primaryInfos1),
            CreateBloxPool("BloxPool145461BloxPoolOne87546", primaryInfos1),
            CreateBloxPool("BloxPool1558646546465464646465", primaryInfos1),
            CreateBloxPool("BloxPool16AQQSDFGHJBHHVHGVHMGV", primaryInfos1, null, 100),
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
