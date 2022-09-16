using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Functionland.FxFiles.Shared.Models
{
    [Table("PinnedArtifact")]
    public class PinnedArtifact
    {
        [Key]
        public int Id { get; set; }
        public string? FullPath { get; set; }
        public string? ThumbnailPath { get; set; }
        public string? ContentHash { get; set; }
        public FsFileProviderType? ProviderType { get; set; }
        public long? PinEpochTime { get; set; }

        // ToDo: Implment this.
        [NotMapped]
        public DateTimeOffset PinDateTime
        {
            get
            {
                return default;
            }
            set
            {

            }
        }
    }
}
