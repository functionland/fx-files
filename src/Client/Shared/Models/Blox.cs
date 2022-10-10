namespace Functionland.FxFiles.Client.Shared.Models;

public class Blox
{
    public string Id { get; set; } = default!;
    public string Name { get; set; } = default!;
    public FulaUser Owner { get; set; } = default!;
    public string? PoolId { get; set; }
    public string? PoolName { get; set; }
    
    public long TotalSpace { get; set; }
    public long FreeSpace { get; set; }
    public long UsedSpace { get; set; }

    public long? PhotosUsed { get; set; }
    public long? VideosUsed { get; set; }
    public long? AudiosUsed { get; set; }
    public long? DocsUsed { get; set; }
    public long? OtherUsed { get; set; }
}