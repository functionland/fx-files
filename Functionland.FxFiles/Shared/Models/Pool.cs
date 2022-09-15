using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Functionland.FxFiles.Shared.Models
{
    public class Pool
    {
        public int? Id { get; set; }
        public string? Name { get; set; }
        public PoolType? PoolType { get; set; }
        public long? LastUpdate { get; set; }
        public decimal? MonthlyRate { get; set; }
        public decimal? CurrentUse { get; set; }
        public float? PhotosUsed { get; set; }
        public float? VideosUsed { get; set; }
        public float? AudiosUsed { get; set; }
        public float? DocsUsed { get; set; }
        public float? OtherUsed { get; set; }
        public string? AdditinalInformation { get; set; } //KeyValuePair<string, string>
    }
}
