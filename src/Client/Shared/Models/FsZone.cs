namespace Functionland.FxFiles.Client.Shared.Models
{
    public class FsZone
    {
        public FsZone(string name)
        {
            var currentEpochTime = DateTimeOffset.Now.ToUnixTimeSeconds();
            Name = name;
            CreateEpochTime = currentEpochTime;
            UpdateEpochTime = currentEpochTime;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public long CreateEpochTime { get; set; }
        public long UpdateEpochTime { get; set; }
        public string? SharedWithDId { get; set; }
    }
}
