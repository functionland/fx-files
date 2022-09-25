using Prism.Events;

namespace Functionland.FxFiles.Client.Shared.Services.Contracts
{
    public class ArtifactChangeEvent : PubSubEvent<ArtifactChangeEvent>
    {
        public FsArtifact? FsArtifact { get; set; }
        public FsArtifactChangesType? ChangeType { get; set; }
        public string? Description { get; set; }
    }
}