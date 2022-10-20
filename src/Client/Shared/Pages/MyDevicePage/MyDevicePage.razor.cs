using Functionland.FxFiles.Client.Shared.Components.Modal;

namespace Functionland.FxFiles.Client.Shared.Pages
{
    public partial class MyDevicePage
    {
        private ArtifactSelectionModal _artifactSelectionModalRef = default!;

        [AutoInject] private ILocalDeviceFileService _fileService { get; set; } = default!;

        [AutoInject] private ILocalDevicePinService _pinService { get; set; } = default!;

        [AutoInject] private InMemoryAppStateStore _artifactState { get; set; } = default!;
    }
}