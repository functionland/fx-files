namespace Functionland.FxFiles.Client.Shared.Services;

public partial class FakeBloxServiceFactory
{
    public FakeBloxService CreateTypical(TimeSpan? actionLatency = null, TimeSpan? enumerationLatency = null)
    {
        var bloxs = new List<Blox>
        {
            new Blox{ Id = "Blox 1",  Name = "Blox 1" }
        };

        var invitedBloxs = new List<Blox>
        {
            new Blox{ Id = "My Friend Blox 1", Name = "My Friend Blox 1" },
            new Blox{ Id = "My Friend Blox 2", Name = "My Friend Blox 2" }
        };

        return new FakeBloxService(bloxs, invitedBloxs);
    }

    public Blox CreateSimpleBlox(TimeSpan? actionLatency = null, TimeSpan? enumerationLatency = null)
    {
        var blox = CreateBlox("The long number of characters in the blox name should be handled ", "Blox 1");
        return blox;
    }

    public FakeBloxService CreateInvitedBloxs(TimeSpan? actionLatency = null, TimeSpan? enumerationLatency = null)
    {
        var fulaUser = new FulaUser("FulaUserDID")
        {
            Username = "NewFulaUser",
            IsParent = false
        };

        var invitedBloxs = new FakeBloxService(
            new List<Blox>
            {
                CreateBlox("Fifth one", "Fifth Blox"),
                CreateBlox("Second Blox", "Second Blox",10000000000, 6000000000, 4000000000, fulaUser, "Second Pool", "Second"),
                CreateBlox("Third Blox", "Third Blox",20000000000, 0, 20000000000, fulaUser, null, null, 0, 0, 0, 0, 0),
                CreateBlox("Fourth Blox", "Fourth Blox",20000000000, 20000000000, 0, fulaUser, null, null, 8000000000, 2000000000, 4000000000, 3000000000, 3000000000),
            },
            actionLatency,
            enumerationLatency);

        return invitedBloxs;
    }

    public FakeBloxService CreateBloxs(TimeSpan? actionLatency = null, TimeSpan? enumerationLatency = null)
    {
        var bloxs = new FakeBloxService(
            new List<Blox>
            {
                CreateBlox("The long number of characters in the blox name should be handled", "My Blox",10000000000, 2000000000, 8000000000, null, null, null, 1000000000, 0, 0, 1000000000, 0),
                CreateBlox("Home Blox", "Home Blox",20000000000, 20000000000, 0,null , null, null, 8000000000, 2000000000, 4000000000, 3000000000, 3000000000),
                CreateBlox("Company Blox", "Company Blox",20000000000, 20000000000, 0,null , null, null, 8000000000, 2000000000, 4000000000, 3000000000, 3000000000)
            },
            actionLatency,
            enumerationLatency);

        return bloxs;
    }
    public FakeBloxService CreateALotOfBloxs(TimeSpan? actionLatency = null, TimeSpan? enumerationLatency = null)
    {
        var fulaUser = new FulaUser("DID")
        {
            Username = "FulaUser",
            IsParent = true
        };

        var bloxs = new FakeBloxService(
            new List<Blox>
            {
                CreateBlox("First Blox", "First Blox"),
                CreateBlox("Second Blox", "Second Blox",null, null, null,fulaUser),
                CreateBlox("Third Blox", "Third Blox",null, null, null,fulaUser),
                CreateBlox("Fourth Blox", "Fourth Blox",20000000000, 8000000000, 12000000000,fulaUser),
                CreateBlox("Fifth Blox", "Fifth Blox",20000000000, 0, 20000000000,fulaUser, null, null, 0, 0, 0, 0, 0),
                CreateBlox("Sixth Blox", "Sixth Blox",20000000000, 20000000000, 0,fulaUser, null, null, 8000000000, 2000000000, 4000000000, 3000000000, 3000000000),
                CreateBlox("Seventh Blox", "Seventh Blox",30000000000, 23000000000, 7000000000,fulaUser, null, null, 11000000000, 5000000000, 4000000000, 1000000000, 2000000000),
                CreateBlox("Eighth Blox", "Eighth Blox",null, null, null,fulaUser, "Eighth Pool", "Eighth"),
                CreateBlox("Canada Blox", "Canada Blox",null, null, null, null, "Canada Pool", "Canada"),
                CreateBlox("Germany Blox", "Germany Blox",10000000000, 3000000000, 7000000000, null, "Germany Pool", "Germany"),
                CreateBlox("China Blox", "China Blox",10000000000, 0, 10000000000, fulaUser, "China Pool", "China"),
                CreateBlox("Home Blox", "Home Blox",10000000000, 6000000000, 4000000000, fulaUser, "Home Pool", "Home"),
                CreateBlox("Company Blox", "Company Blox",10000000000, 10000000000, 0, fulaUser, "Company Pool", "Company"),
                CreateBlox("My City Blox", "My City Blox",30000000000, 22000000000, 8000000000,fulaUser, null, null, 10000000000, 5000000000, 4000000000, 1000000000, 2000000000)
            },
            new List<Blox>
            {
                CreateBlox("My City InvitedBloxs", "My City Blox",10000000000, 0, 0, fulaUser, "My City", "My City Pool"),
                CreateBlox("My Father InvitedBloxs", "My Father Blox",10000000000, 5000000000, 5000000000, fulaUser, "12364"),
                CreateBlox("My Friend InvitedBloxs", "My Friend Blox",10000000000, 2000000000, 8000000000,fulaUser, null, null, 1000000000, 0, 0, 1000000000, 0),
                CreateBlox("The long number of characters in the InvitedBloxs name should be handled ", "InvitedBloxs")
            },
            actionLatency,
            enumerationLatency);

        return bloxs;
    }

    private static Blox CreateBlox(string bloxId,
                                  string name,
                                  long? totalSpace = null,
                                  long? usedSpace = null,
                                  long? freeSpace = null,
                                  FulaUser owner = null,
                                  string? poolName = null,
                                  string? poolId = null,
                                  long? photosUsed = null,
                                  long? videosUsed = null,
                                  long? audiosUsed = null,
                                  long? docsUsed = null,
                                  long? otherUsed = null)
    {
        var blox = new Blox()
        {
            Id = bloxId,
            Name = name,
            TotalSpace = totalSpace,
            UsedSpace = usedSpace,
            FreeSpace = freeSpace,
            Owner = owner,
            PoolName = poolName,
            PoolId = poolId,
            PhotosUsed = photosUsed,
            VideosUsed = videosUsed,
            AudiosUsed = audiosUsed,
            DocsUsed = docsUsed,
            OtherUsed = otherUsed
        };

        return blox;
    }
}
