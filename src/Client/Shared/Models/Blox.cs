namespace Functionland.FxFiles.Client.Shared.Models;

public class Blox
{
    public Blox(string id, string name, string ownerName)
    {
        Id = id;
        Name = name;
        OwnerName = ownerName;
    }

    public string Id { get; set; }
    public string Name { get; set; }
    public string OwnerName { get; set; }
    public string? PoolId { get; set; }
}