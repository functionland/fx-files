namespace Functionland.FxFiles.Client.Shared.Models
{
    public class Blox
    {
        public int? Id { get; set; }
        public string? Name { get; set; }
        public string? OwnerId { get; set; }
        public string? PoolName { get; set; }
        public float? AvailableSpace { get; set; }
        public float? CurrentUse { get; set; }
        public float? PhotosUsed { get; set; }
        public float? VideosUsed { get; set; }
        public float? AudiosUsed { get; set; }
        public float? DocsUsed { get; set; }
        public float? OtherUsed { get; set; }
    }
}
