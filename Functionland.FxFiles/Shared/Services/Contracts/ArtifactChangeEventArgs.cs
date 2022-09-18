using Prism.Events;

namespace Functionland.FxFiles.Shared.Services.Contracts
{
    public class ArtifactChangeEventArgs: PubSubEvent<ArtifactChangeEventArgs>
    {
        public FsArtifact? FsArtifact { get; set; }
        public FsArtifactChangesType? ChangeType { get; set; }
    }
}