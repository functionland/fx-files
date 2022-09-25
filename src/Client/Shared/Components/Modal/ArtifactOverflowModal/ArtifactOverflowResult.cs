namespace Functionland.FxFiles.Client.Shared.Components.Modal
{
    public class ArtifactOverflowResult
    {
        public ArtifactOverflowResultType ResultType { get; set; }

        public IEnumerable<FsArtifact> SelectedArtifacts { get; set; } = default!;
    }
}
