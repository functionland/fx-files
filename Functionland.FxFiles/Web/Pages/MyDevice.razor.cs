using Functionland.FxFiles.App.Components.Common;
using Functionland.FxFiles.App.Components.Modal;

namespace Functionland.FxFiles.App.Pages
{
    public partial class MyDevice
    {
        private ArtifactSelectionModal _artifactSelectionModalRef = default!;

        [AutoInject] private IFileService _fileService { get; set; } = default!;

        [AutoInject] private IPinService _pinService { get; set; } = default!;
    }
}
