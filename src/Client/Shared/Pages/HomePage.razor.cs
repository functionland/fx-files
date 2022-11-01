using Functionland.FxFiles.Client.Shared.Components.Modal;
using System.Net;

namespace Functionland.FxFiles.Client.Shared.Pages;

public partial class HomePage
{
    [AutoInject] public IntentHolder IntentHolder { get; set; } = default!;
    [AutoInject] public ILocalDeviceFileService LocalDeviceFileService { get; set; } = default!;
    private FileViewer? _fileViewerRef;
    protected override async Task OnInitAsync()
    {
        await base.OnInitAsync();
        if (IntentHolder.FileUrl is not null)
            return;

        if (IsiOS)
            NavigationManager.NavigateTo("settings", false, true);
        else
            NavigationManager.NavigateTo("mydevice", false, true);
    }


    protected async override Task OnAfterFirstRenderAsync()
    {
        await base.OnAfterFirstRenderAsync();

        if (IntentHolder.FileUrl is not null)
        {
            var artifact = await LocalDeviceFileService.GetArtifactAsync(IntentHolder.FileUrl);
            IntentHolder.FileUrl = null;
            var isOpen = _fileViewerRef.OpenArtifact(artifact);
        }
    }
}