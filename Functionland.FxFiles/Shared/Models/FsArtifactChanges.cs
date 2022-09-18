using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Functionland.FxFiles.Shared.Models
{
    public class FsArtifactChanges
    {
        public string? ArtifactFullPath { get;set; }
        public FsArtifactChangesType? FsArtifactChangesType { get; set; }
        public DateTimeOffset LastModifiedDateTime { get; set; }
        public bool? IsPathExist { get; set; }
    }
}
