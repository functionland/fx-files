namespace Functionland.FxFiles.Client.Shared.Components
{
    public class ArtifactActionResult
    {
        public ArtifactActionType ActionType { get; set; }
        public List<FsArtifact>? Artifacts { get; set; }
    }
}
