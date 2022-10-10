using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Functionland.FxFiles.Client.Shared.Components.Modal
{
    public class ProgressInfo
    {
        public string? CurrentText { get; set; }
        public string? CurrentSubText { get; set; }
        public int? CurrentValue { get; set; }
        public int? MaxValue { get; set; }
    }
}
