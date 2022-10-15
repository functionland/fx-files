namespace Functionland.FxFiles.Client.Shared.Services.Implementations.IdentityService;

public class FakeIdentityService : IIdentityService
{
    private readonly List<KeyValuePair<FulaUser, List<FulaUser>?>> _fulaUsers;

    private FulaUser? _currentUser;
    public TimeSpan? ActionLatency { get; set; }
    public TimeSpan? EnumerationLatency { get; set; }

    public FakeIdentityService(IEnumerable<KeyValuePair<FulaUser, List<FulaUser>?>>? fulaUsers = null,
                               FulaUser? currentUser = null,
                               TimeSpan? actionLatency = null,
                               TimeSpan? enumerationLatency = null)
    {
        _fulaUsers ??= new List<KeyValuePair<FulaUser, List<FulaUser>?>>(); 
        _fulaUsers.Clear();
        _currentUser = currentUser;
        ActionLatency = actionLatency ?? TimeSpan.FromSeconds(2);
        EnumerationLatency = enumerationLatency ?? TimeSpan.FromMilliseconds(10);

        if (fulaUsers is not null)
        {
            foreach (var fulaUser in fulaUsers)
            {
                _fulaUsers.Add(fulaUser);
            }
        }
        else
        {
            _fulaUsers = new List<KeyValuePair<FulaUser, List<FulaUser>?>>();
        }
    }

    public async Task<List<FulaUser>> LoginAsync(DIdDocument dIdDocument, string securityKey, CancellationToken? cancellationToken = null)
    {
        if (ActionLatency != null)
        {
            await Task.Delay(ActionLatency.Value);
        }

        var parent = _fulaUsers?.FirstOrDefault(a => a.Key.DId == dIdDocument.DId && a.Key.IsParent == true).Key;
        var children = _fulaUsers?.FirstOrDefault(a => a.Key.DId == dIdDocument.DId).Value;

        if (parent is null)
            throw new Exception(""); //TODO

        List<FulaUser> fulaUsers = new()
        {
            parent
        };

        if (children is null) return fulaUsers;

        foreach (var child in children)
        {
            fulaUsers.Add(child);
        }

        return fulaUsers;
    }

    public void SetCurrentUser(FulaUser user)
    {
        _currentUser = user;
    }

    public async Task<FulaUser> GetCurrentUserAsync(CancellationToken? cancellationToken = null)
    {
        if (ActionLatency != null)
        {
            await Task.Delay(ActionLatency.Value);
        }

        if (_currentUser is null)
            throw new Exception(""); //TODO

        return _currentUser;
    }

    public async Task<FulaUser> RefreshUserAsync(DIdDocument dIdDocument, CancellationToken? cancellationToken = null)
    {
        if (ActionLatency != null)
        {
            await Task.Delay(ActionLatency.Value);
        }

        var user = _fulaUsers?.FirstOrDefault(a => a.Key.DId == dIdDocument.DId).Key; //?????????

        if (user is null)
            throw new Exception(""); //TODO

        return user;
    }

    public async Task<bool> IsLoggedInAsync(CancellationToken? cancellationToken = null)
    {
        if (ActionLatency != null)
        {
            await Task.Delay(ActionLatency.Value);
        }

        if (_currentUser is not null)
            return true;

        return false;
    }

    public Task<DIdDocument> ConnectToWalletAsync(Uri uri, CancellationToken? cancellationToken = null)
    {
        throw new NotImplementedException();
    }

    public async Task<FulaUser> GetUserAsync(string dId, CancellationToken? cancellationToken = null)
    {
        if (ActionLatency != null)
        {
            await Task.Delay(ActionLatency.Value);
        }

        var fulaUser = GetUser(dId);

        if (fulaUser is null)
            throw new Exception(""); //TODO

        return fulaUser;
    }

    public async Task<List<FulaUser>> GetUsersAsync(IEnumerable<string> dids, CancellationToken? cancellationToken = null)
    {
        var fulaUsers = new List<FulaUser>();

        foreach (var did in dids)
        {
            if (EnumerationLatency is not null)
            {
                await Task.Delay(EnumerationLatency.Value);
            }

            var user = GetUser(did);

            if(user is not null)
            {
                fulaUsers.Add(user);
            }
        }

        return fulaUsers;
    }

    private FulaUser? GetUser(string dId, CancellationToken? cancellationToken = null)
    {
        if (_fulaUsers is null)
            throw new Exception(""); //TODO

        var fulaUser = _fulaUsers?.Where(a => a.Key.DId == dId).FirstOrDefault().Key;

        if (fulaUser is not null)
            return fulaUser;

        if(_fulaUsers is not null)
        {
            foreach (var parentUser in _fulaUsers.Select(c => c.Value))
            {

                if (parentUser is null)
                    continue;

                foreach (var child in parentUser)
                {
                    if (child.DId == dId)
                    {
                        fulaUser = child;
                    }
                }
            }
        }
        
        return fulaUser;
    }

    public async Task<Stream> GetAvatarAsync(string did, CancellationToken? cancellationToken = null)
    {
        if (ActionLatency != null)
        {
            await Task.Delay(ActionLatency.Value);
        }

        using FileStream stream = File.Open("/images/image germany.jpg", FileMode.Open);

        return stream;
    }

    public async Task<Stream> GetMyAvatarAsync(CancellationToken? cancellationToken = null)
    {
        if (ActionLatency != null)
        {
            await Task.Delay(ActionLatency.Value);
        }

        using FileStream stream = File.Open("/images/image germany.jpg", FileMode.Open);

        return stream;
    }
    public async Task<string> GetAvatarThumbnailUrlAsync(string did, CancellationToken? cancellationToken = null)
    {
        if (ActionLatency != null)
        {
            await Task.Delay(ActionLatency.Value);
        }

        string imageLink = "https://www.talab.org/wp-content/uploads/2018/02/1410336844-talab-org.jpg";

        return imageLink;
    }
}
