namespace Functionland.FxFiles.App.Components.Modal
{
    public class ArtifactDetailModalResult
    {
        public ArtifactDetailModalResultType ResultType { get; set; }

        public IEnumerable<FsArtifact> SelectedArtifacts { get; set; } = default!;
    }
}
