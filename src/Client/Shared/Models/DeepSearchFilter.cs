
namespace Functionland.FxFiles.Client.Shared.Models;

public struct DeepSearchFilter
{
    public List<ArtifactCategorySearchType>? ArtifactCategorySearchTypes { get; set; }
    public ArtifactDateSearchType? ArtifactDateSearchType { get; set; }
    public string? SearchText { get; set; }

    public bool IsEmpty()
    {
        return string.IsNullOrWhiteSpace(SearchText)
               && ArtifactDateSearchType == null
               && (ArtifactCategorySearchTypes?.Count ?? 0) == 0;
    }
}
