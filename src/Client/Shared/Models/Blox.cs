namespace Functionland.FxFiles.Client.Shared.Models;

public class Blox
{
    public Blox(string name,string ownerId)
    {
        Name = name;
        OwnerId = ownerId;
    }

    public int? Id { get; set; }
    public string Name { get; set; }
    public string OwnerId { get; set; }
    public string? PoolName { get; set; }
    public long AvailableSpace { get; set; }
    public long CurrentUse { get; set; }
    public long PhotosUsed { get; set; }
    public long VideosUsed { get; set; }
    public long AudiosUsed { get; set; }
    public long DocsUsed { get; set; }
    public long OtherUsed { get; set; }
}
