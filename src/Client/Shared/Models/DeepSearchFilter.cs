
namespace Functionland.FxFiles.Client.Shared.Models;

public struct DeepSearchFilter
{
    public List<ArtifactCategorySearchType>? ArtifactCategorySearchTypes { get; set; }
    public ArtifactDateSearchType? ArtifactDateSearchType { get; set; }
    public string? SearchText { get; set; }
}
