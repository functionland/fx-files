namespace Functionland.FxFiles.Client.Shared.Models
{
    public class FsArtifactChanges
    {
        public string? ArtifactFullPath { get; set; }
        public FsArtifactChangesType? FsArtifactChangesType { get; set; }
        public DateTimeOffset LastModifiedDateTime { get; set; }
        public bool? IsPathExist { get; set; }
    }
}
