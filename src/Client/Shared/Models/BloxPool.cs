namespace Functionland.FxFiles.Client.Shared.Models;

public class BloxPool
{
    public BloxPool(string id)
    {
        Id = id;
    }

    /// <summary>
    /// It's a unique name
    /// </summary>
    public string Id { get; set; }

    public int PingTime { get; set; }

    public List<KeyValuePair<string, string>>? PrimaryInfos { get; set; }

    public List<KeyValuePair<string, KeyValuePair<string, string>>>? KeyValueGroups { get; set; }
}
