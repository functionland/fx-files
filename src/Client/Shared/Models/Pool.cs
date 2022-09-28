namespace Functionland.FxFiles.Client.Shared.Models;

public class Pool
{
    public Pool(string name, PoolType poolType)
    {
        Name = name;
        PoolType = poolType;
    }
    public int? Id { get; set; }
    public string Name { get; set; }
    public PoolType PoolType { get; set; }
    public long? UpdateEpochTime { get; set; }
    public decimal? MonthlyRate { get; set; }
    public long CurrentUse { get; set; }
    public long PhotosUsed { get; set; }
    public long VideosUsed { get; set; }
    public long AudiosUsed { get; set; }
    public long DocsUsed { get; set; }
    public long OtherUsed { get; set; }
    public string? AdditinalInformation { get; set; } //KeyValuePair<string, string>
}
