using Prism.Events;

namespace Functionland.FxFiles.Shared.Services.Contracts
{
    public class ArtifactChangeEvent: PubSubEvent<ArtifactChangeEvent>
    {
        public FsArtifact? FsArtifact { get; set; }
        public FsArtifactChangesType? ChangeType { get; set; }
    }
}