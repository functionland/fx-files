namespace Functionland.FxFiles.Client.Shared.Models;

public class UserToken
{
    public UserToken(string token)
    {
        Token = token;
    }
    public string Token { get; set; }
    public List<KeyValuePair<string, string>> Claims { get; set; } = new List<KeyValuePair<string, string>>();
}