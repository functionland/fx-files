using Functionland.FxFiles.Client.Shared.Utils;

namespace Functionland.FxFiles.Client.Shared.Models;

public class FsArtifact
{
    public FsArtifact(string fullPath, string name, FsArtifactType artifactType, FsFileProviderType providerType)
    {
        FullPath = fullPath;
        Name = name;
        ArtifactType = artifactType;
        ProviderType = providerType;

        if (providerType != FsFileProviderType.Fula)
        {
            LocalFullPath = fullPath;
        }
    }

    public long? Id { get; set; }
    public string Name { get; set; }

    public string FullPath { get; set; }
    public string? LocalFullPath { get; set; }
    public string? ParentFullPath { get; set; }
    public string? ThumbnailPath { get; set; }

    public string? FileExtension { get; set; }
    public FsArtifactType ArtifactType { get; set; }
    public FsFileProviderType ProviderType { get; set; }
    public long? Size { get; set; }
    public long? Capacity { get; set; }
    public string? ContentHash { get; set; }
    public string? OriginDevice { get; set; }
    public DateTimeOffset CreateDateTime { get; set; }
    public DateTimeOffset LastModifiedDateTime { get; set; }
    public string? WhoMadeLastEdit { get; set; }
    public List<FsArtifactActivity>? FsArtifactActivity { get; set; }
    public List<ArtifactPermissionInfo>? PermissionUsers { get; set; }
    public FulaUser? Owner { get; set; }
    public ArtifactPermissionLevel ArtifactPermissionLevel { get; set; }

    public bool? IsAvailableOfflineRequested { get; set; }
    public bool? IsSharedWithMe { get; set; }
    public bool? IsSharedByMe { get; set; }

    public ArtifactPersistenceStatus PersistenceStatus { get; set; }
    public string? DId { get; set; }

    // For UI

    public bool? IsPinned { get; set; }
    public bool? IsDisabled { get; set; }
    public string? SizeStr => FsArtifactUtils.CalculateSizeStr(Size);
    public FileCategoryType FileCategory => FsArtifactUtils.GetCategoryType(FileExtension?.ToLower() ?? "");

    public override string ToString()
    {
        return $"{Name} -> {FullPath}";
    }
    public bool? IsSelected { get; set; }
    public string? IntentType { get; set; }
    public bool CanShowBreadcrumb { get; set; } = true;

}