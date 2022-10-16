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
                    CreateBlox("My Blox","The long number of characters in the blox id should be handled ", "OwnerDId")
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
            bloxs,
            new List<Blox>
            {
                CreateBlox("Fifth one", "Fifth Blox", "The long number of characters in the OwnerDId should be handled"),
                CreateBlox("Second Blox", "Second Blox","Second Blox OwnerDId"),
                CreateBlox("The long number of characters in the blox id should be handled", "Third Blox","Third Blox OwnerDId")
            },
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
                CreateBlox("The long number of characters in the blox id should be handled", "The long number of characters in the Blox name should be handled","Blox OwnerDId"),
                CreateBlox("Home Blox Id", "Home Blox Name", "Home Blox Owner"),
                CreateBlox("Company Blox", "Company Blox","Company Blox OwnerDId")
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
                CreateBlox("My Country Blox One", "First Blox", "First Blox OwnerDId"),
                CreateBlox("My Country Blox Two", "Second Blox","Second Blox OwnerDId"),
                CreateBlox("My Country Blox Three", "Third Blox","Third Blox OwnerDId"),
                CreateBlox("My City Blox1", "My City  Blox1", "My City  Blox OwnerDId"),
                CreateBlox("Eighth Blox One", "Eighth Blox One", "Eighth OwnerDId"),
                CreateBlox("Eighth Blox Two", "Eighth Blox Two", "Eighth OwnerDId"),
                CreateBlox("Canada Blox", "Canada Blox", "Canada OwnerDId"),
                CreateBlox("Germany Blox", "Germany Blox", "Germany OwnerDId"),
                CreateBlox("China Blox", "China Blox", "China OwnerDId"),
                CreateBlox("Home Blox", "Home Blox", "Home OwnerDId"),
                CreateBlox("Company Blox", "Company Blox", "Company OwnerDId")
            },
            new List<Blox>
            {
                CreateBlox("My City InvitedBloxs", "My City Blox", "My City"),
                CreateBlox("My Father InvitedBloxs", "My Father Blox","OwnerDId"),
                CreateBlox("My Friend InvitedBloxs", "My Friend Blox", "1452Canada"),
                CreateBlox("The long number of characters in the InvitedBloxs name should be handled ", "InvitedBloxs", "Germany"),
                CreateBlox("Company InvitedBloxs", "Company InvitedBloxs", "Company"),
                CreateBlox("China InvitedBloxs", "China InvitedBloxs", "ChinaInvitedBloxs"),
            },
            actionLatency,
            enumerationLatency);

        return bloxs;
    }

    private static Blox CreateBlox(string bloxId,
                                   string name,
                                   string ownerDId)
    {
        var blox = new Blox(bloxId, name, ownerDId);

        return blox;
    }
}
