namespace Functionland.FxFiles.Client.Shared.Models
{
    public class BloxPool
    {
        public int? Id { get; set; }
        public string? Name { get; set; }
        public PoolType? PoolType { get; set; }
        public long? LastUpdate { get; set; }
        public decimal? MonthlyRate { get; set; }
        public decimal? CurrentUse { get; set; }
        public long? PhotosUsed { get; set; }
        public long? VideosUsed { get; set; }
        public long? AudiosUsed { get; set; }
        public long? DocsUsed { get; set; }
        public long? OtherUsed { get; set; }
        public List<KeyValuePair<string, string>> AdditinalInformation { get; set; } = new List<KeyValuePair<string, string>>()
    }
}
