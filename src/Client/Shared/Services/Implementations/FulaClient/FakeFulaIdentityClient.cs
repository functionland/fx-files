using Functionland.FxFiles.Client.Shared.Utils;

namespace Functionland.FxFiles.Client.Shared.Services.Implementations;

public class FakeFulaIdentityClient : IFulaIdentityClient
{
    /// <summary>
    /// key is user token
    /// </summary>
    private readonly Dictionary<string, Dictionary<FulaUser, Stream>> _users;
    public FakeFulaIdentityClient(Dictionary<string, Dictionary<FulaUser, Stream>> users)
    {
        _users = users;
    }
    public async Task<Stream> GetAvatarAsync(string token, string did, CancellationToken? cancellationToken = null)
    {
        var decodedDId = FulaUserUtils.GetFulaDId(token);
        if (!did.Equals(decodedDId)) throw new Exception("Authotization failed");

        var users = _users[token];

        if (!users.Any()) throw new Exception("Authotization failed");

        var user = users.Where(c => c.Key.DId.Equals(did)).FirstOrDefault().Value;

        if (user == null) throw new Exception("Authotization failed");

        return user;
    }

    public async Task<List<FulaUser>> GetUsersAsync(string token, IEnumerable<string> otherDids, CancellationToken? cancellationToken = null)
    {
        var decodedDId = FulaUserUtils.GetFulaDId(token);
        if (string.IsNullOrWhiteSpace(decodedDId)) throw new Exception("Authotization failed");

        var users = _users[token];

        if (!users.Any()) throw new Exception("Authotization failed");

        var result = new List<FulaUser>();

        foreach (var item in _users)
        {
            var values = item.Value;

            foreach (var value in values)
            {
                var user = value.Key;
                if (otherDids.Contains(user.DId))
                {
                    result.Add(user);
                }
            }
        }

        return result;
    }

    public async Task<UserToken> LoginAsync(string dId, string securityKey, CancellationToken? cancellationToken = null)
    {
        var token = FulaUserUtils.CreateToken(dId, securityKey);

        if (!_users.ContainsKey(token))
        {
            var fulaUser = new FulaUser(dId);
            var dic = new Dictionary<FulaUser, Stream>();
            using FileStream fs = File.Open("/Files/fake-pic.jpg", FileMode.Open);
            dic.Add(fulaUser, fs);
            _users.Add(token, dic);
        }

        return new UserToken(token);
    }
}
