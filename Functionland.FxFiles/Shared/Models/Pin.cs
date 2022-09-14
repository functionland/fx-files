using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Functionland.FxFiles.Shared.Models
{
    [Table("Pin")]
    public class Pin
    {
        [Key]
        public int Id { get; set; }
        public string? ArtifactLocalPath { get; set; }
        public string? ArtifactPath { get; set; }
        public FsFileProviderType? ProviderType { get; set; }
        public long? PinDateTime { get; set; }
    }
}
