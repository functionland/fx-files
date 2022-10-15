namespace Functionland.FxFiles.Client.Shared.Models;

public class Blox
{
    public Blox(string id, string name, string ownerDId)
    {
        Id = id;
        Name = name;
        OwnerDId = ownerDId;
    }

    public string Id { get; set; }
    public string Name { get; set; }
    public string OwnerDId { get; set; }
}