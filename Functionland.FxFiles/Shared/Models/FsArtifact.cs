using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Functionland.FxFiles.Shared.Models
{
    public class FsArtifact
    {
        public int Id { get; set; }
        public string? FullPath { get; set; }
        public string? Name { get; set; }
        public string? FileExtension { get; set; }
        public FxArtifactType? ArtifactType { get; set; }
        public FxFileProviderType? ProviderType { get; set; }
        int? Size { get; set; }
        int? Capacity { get; set; }
        public string? ContentHash { get; set; }
        public int? ParentId { get; set; }
        public string? OriginDevice { get; set; }
        public DateTimeOffset LastModifiedDateTime { get; set; }
        public string? ThumbnailPath { get; set; }
        // For UI
        public List<FsZone>? Zones { get; set; }
        public bool? IsSharedWithMe { get; set; }
        public bool? IsSharedByMe { get; set; }
        public string? OwnerDid { get; set; }
        public string? IsAvailableOffline { get; set; }
        public bool? IsPinned { get; set; }
    }


}
