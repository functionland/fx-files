namespace Functionland.FxFiles.Client.Shared.Services;

public partial class FakeBloxServiceFactory
{
    public FakeBloxService CreateTypical(TimeSpan? actionLatency = null, TimeSpan? enumerationLatency = null)
    {
        var blox = new FakeBloxService(
                new List<Blox>
                {
                    CreateBlox("Blox 1","Blox 1","Blox 1")
                },
                new List<Blox>
                {
                    CreateBlox("My Friend Blox 1","My Friend Blox 1","My Friend Blox 1"),
                    CreateBlox("My Friend Blox 2","My Friend Blox 2","My Friend Blox 2")
                },
                actionLatency,
                enumerationLatency);
      
        return blox;
    }

    public FakeBloxService CreateSimpleBlox(TimeSpan? actionLatency = null, TimeSpan? enumerationLatency = null)
    {
        var invitedBloxs = new List<Blox>();

        var blox = new FakeBloxService(
                new List<Blox>
                {
                    CreateBlox("My Blox","The long number of characters in the blox id should be handled ", "Owner Name","1426")
                },
                invitedBloxs,
                actionLatency,
                enumerationLatency);

        return blox;
    }

    public FakeBloxService CreateInvitedBloxs(TimeSpan? actionLatency = null, TimeSpan? enumerationLatency = null)
    {
        var bloxs = new List<Blox>();
        var invitedBloxs = new FakeBloxService(
            new List<Blox>
            {
                CreateBlox("Fifth one", "Fifth Blox", "The long number of characters in the owner blox name should be handled"),
                CreateBlox("Second Blox", "Second Blox","The long number of characters in the Owner name should be handled","Second Pool"),
                CreateBlox("The long number of characters in the blox id should be handled", "Third Blox","Second Pool", "Second")
            },
            bloxs,
            actionLatency,
            enumerationLatency);

        return invitedBloxs;
    }

    public FakeBloxService CreateBloxs(TimeSpan? actionLatency = null, TimeSpan? enumerationLatency = null)
    {
        var invitedBloxs = new List<Blox>();

        var bloxs = new FakeBloxService(
            new List<Blox>
            {
                CreateBlox("The long number of characters in the blox id should be handled", "The long number of characters in the Blox name should be handled","Blox Owner"),
                CreateBlox("Home Blox Id", "Home Blox Name", "Home Blox Owner", "Home Blox PoolId"),
                CreateBlox("Company Blox", "Company Blox","The long number of characters in the owner blox name should be handled","Company Blox PoolId")
            },
            invitedBloxs,
            actionLatency,
            enumerationLatency);

        return bloxs;
    }

    public FakeBloxService CreateALotOfBloxs(TimeSpan? actionLatency = null, TimeSpan? enumerationLatency = null)
    {
        var bloxs = new FakeBloxService(
            new List<Blox>
            {
                CreateBlox("My Country Blox One", "First Blox", "First Blox Owner" , "First"),
                CreateBlox("My Country Blox Two", "Second Blox","Second Blox Owner", "Second"),
                CreateBlox("My Country Blox Three", "Third Blox","Third Blox Owner", "Third"),
                CreateBlox("My City Blox1", "My City  Blox1", "My City  Blox Owner", "PoolId 12365"),
                CreateBlox("Eighth Blox One", "Eighth Blox One", "Eighth Owner Name", "Eighth11111"),
                CreateBlox("Eighth Blox Two", "Eighth Blox Two", "Eighth Owner Name", "Eighth12222"),
                CreateBlox("Canada Blox", "Canada Blox", "Canada Owner Name", "Canada"),
                CreateBlox("Germany Blox", "Germany Blox", "Germany Owner Name", "Germany"),
                CreateBlox("China Blox", "China Blox", "China Owner Name", "China"),
                CreateBlox("Home Blox", "Home Blox", "Home Owner Name", "Home"),
                CreateBlox("Company Blox", "Company Blox", "Company Owner Name", "Company")
            },
            new List<Blox>
            {
                CreateBlox("My City InvitedBloxs", "My City Blox", "My City", "My City Pool"),
                CreateBlox("My Father InvitedBloxs", "My Father Blox", "12364"),
                CreateBlox("My Friend InvitedBloxs", "My Friend Blox","Canada", "1452Canada"),
                CreateBlox("The long number of characters in the InvitedBloxs name should be handled ", "InvitedBloxs", "Germany" , "1254Germany"),
                CreateBlox("Company InvitedBloxs", "Company InvitedBloxs", "Company Owner Name", "Company"),
                CreateBlox("China InvitedBloxs", "China InvitedBloxs", "China InvitedBloxs Owner Name", "ChinaInvitedBloxs"),
            },
            actionLatency,
            enumerationLatency);

        return bloxs;
    }

    private static Blox CreateBlox(string bloxId,
                                  string name,
                                  string ownerName,
                                  string? poolId = null)
    {
        var blox = new Blox(bloxId, name, ownerName)
        {
            PoolId = poolId,
        };

        return blox;
    }
}
