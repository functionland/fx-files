using Functionland.FxFiles.App.Components.Common;

namespace Functionland.FxFiles.App.Pages
{
    public partial class MyDevice
    {

        [AutoInject] private IFileService _fileService { get; set; } = default!;
        
        [AutoInject] private IPinService _pinService { get; set; } = default!;
    }
}
