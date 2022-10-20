namespace Functionland.FxFiles.Client.Shared.Services.Implementations.IdentityService;

public partial class FakeIdentityServiceFactory
{
    public FakeIdentityService CreateCurrentUser(TimeSpan? actionLatency = null, TimeSpan? enumerationLatency = null)
    {
        List<FulaUser> currentUserChildren = new()
        {
            FulaUser("First Child DId", "First Child", false, true ),
            FulaUser("Second Child DId", "Second Child", false, true ),
            FulaUser("Third Child DId", "Third Child", false, false ),
        };

        var fulaUsers = new FakeIdentityService(
            new List<KeyValuePair<FulaUser, List<FulaUser>?>>
            {
                 new KeyValuePair<FulaUser, List<FulaUser>?>(FulaUser("The first member of fula", "NewUser", true, true ), currentUserChildren)
            },
            FulaUser("The first member of fula", "NewUser", true, true),
            actionLatency,
            enumerationLatency);

        return fulaUsers;
    }

    public FakeIdentityService CreateFulaUsers(TimeSpan? actionLatency = null, TimeSpan? enumerationLatency = null)
    {
        List<FulaUser> children = new()
        {
            FulaUser("First Child DId", "First Child", false, true ),
            FulaUser("Second Child DId", "Second Child", false, true ),
            FulaUser("Third Child DId", "Third Child", false, false ),
        };

        List<FulaUser> currentUserChildren = new()
        {
            FulaUser("Current User Child 123", "First Child", false, true ),
            FulaUser("Current User Child 223", "First Child", false, false )
        };

        var fulaUsers = new FakeIdentityService(
            new List<KeyValuePair<FulaUser, List<FulaUser>?>>
            {
                 new KeyValuePair<FulaUser, List<FulaUser>?>(FulaUser("Current User DId", "FulaUser", true, true ), currentUserChildren),
                 new KeyValuePair<FulaUser, List<FulaUser>?>(FulaUser("New User DId 125452", "New User One", true, true ), null),
                 new KeyValuePair<FulaUser, List<FulaUser>?>(FulaUser("Fula User DId 1452789547", "FulaUser Two", true, true ), children),
            },
            FulaUser("Current User DId", "FulaUser", true, true ),
            actionLatency,
            enumerationLatency);

        return fulaUsers;
    }



    private FulaUser FulaUser(string dId, string username, bool isParent, bool hasAceessToFula)
    {
        var fulaUser = new FulaUser(dId)
        {
            Username = username,
            IsParent = isParent
        };
        return fulaUser;
    }
}
