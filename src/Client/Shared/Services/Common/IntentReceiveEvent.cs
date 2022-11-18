using Prism.Events;

namespace Functionland.FxFiles.Client.Shared.Services.Common;

public class IntentReceiveEvent: PubSubEvent<IntentReceiveEvent>
{
    public bool MustOpenCurrentArtifact { get; set; }
}
