using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Functionland.FxFiles.Client.Shared.Models
{
    [Dapper.Contrib.Extensions.Table("FsArtifactTable")]
    public class FsArtifactTable
    {
        // TODO: Check table name.
        // TODO: Check the added properties and their types.

        [Key]
        public long Id { get; set; }
        public string FullPath { get; set; }
        public string LocalFullPath { get; set; }
        public string Name { get; set; }
        public string? FileExtension { get; set; }
        public FsArtifactType ArtifactType { get; set; }
        public FsFileProviderType ProviderType { get; set; }
        public long? Size { get; set; }
        public long? Capacity { get; set; }
        public string? ContentHash { get; set; }
        public string? ParentFullPath { get; set; }
        public string? OriginDevice { get; set; }
        public DateTimeOffset CreateDateTime { get; set; }
        public DateTimeOffset LastModifiedDateTime { get; set; }
        public string? WhoMadeLastEdit { get; set; }
        public string? ThumbnailPath { get; set; }
        public ArtifactPersistenceStatus PersistenceStatus { get; set; }
        public string DId { get; set; }
        public bool? IsAvailableOfflineRequested { get; set; }
        public bool? IsSharedWithMe { get; set; }
        public bool? IsSharedByMe { get; set; }
    }
}
