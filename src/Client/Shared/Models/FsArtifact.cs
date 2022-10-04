using Functionland.FxFiles.Client.Shared.Utils;

namespace Functionland.FxFiles.Client.Shared.Models
{
    public class FsArtifact
    {
        public FsArtifact(string fullPath, string name, FsArtifactType artifactType, FsFileProviderType providerType)
        {
            FullPath = fullPath;
            Name = name;
            ArtifactType = artifactType;
            ProviderType = providerType;
        }

        public long? Id { get; set; }
        public string FullPath { get; set; }
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
        public string LastModifiedByWho { get; set; }
        public string? ThumbnailPath { get; set; }
        // For UI
        public string LocalFullPath { get; set; }
        public bool? IsSharedWithMe { get; set; }
        public bool? IsSharedByMe { get; set; }
        public string? OwnerDid { get; set; }
        public string? IsAvailableOffline { get; set; }
        public bool? IsPinned { get; set; }
        public string? SizeStr => FsArtifactUtils.CalculateSizeStr(Size);
        public FileCategoryType FileCategory => FsArtifactUtils.GetCategoryType(FileExtension ?? "");
    }
}