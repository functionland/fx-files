using Functionland.FxFiles.Client.Shared.Components.Modal;

namespace Functionland.FxFiles.Client.Shared.Pages;

public partial class HomePage
{
    [AutoInject] public ILocalDeviceFileService LocalDeviceFileService { get; set; } = default!;
    private FileViewer? _fileViewerRef;
    protected override async Task OnInitAsync()
    {
        await base.OnInitAsync();

        if (IsiOS)
            NavigationManager.NavigateTo("settings", false, true);
        else
            NavigationManager.NavigateTo("mydevice", false, true);
    }
}