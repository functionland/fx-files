namespace Functionland.FxFiles.Client.Shared.Models;

public class FulaUser
{
    public FulaUser(string dId)
    {
        DId = dId;
    }

    public string DId { get; set; }
    public string? Username { get; set; }
    public bool IsParent { get; set; }
    public List<KeyValuePair<string, string>> Claims { get; set; } = new List<KeyValuePair<string, string>>();

    public bool HasAceessToFula
    {
        get
        {
            var hasAceessToFulaClaim = Claims.Where(c => c.Key.ToLower().Equals("hasaceessyofula")).Select(c => c.Value).FirstOrDefault();
            return hasAceessToFulaClaim != null && hasAceessToFulaClaim.ToLower() == "true";
        }
    }
}
