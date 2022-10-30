using System.Net;

namespace Functionland.FxFiles.Client.Shared.Pages;

public partial class HomePage
{
    [AutoInject] public IntentHolder IntentHolder { get; set; } = default!;
    [AutoInject] public IViewFileService<ILocalDeviceFileService> ViewFileService { get; set; } = default!;
    protected override async Task OnInitAsync()
    {
        await base.OnInitAsync();

        if (IsiOS)
            NavigationManager.NavigateTo("settings", false, true);
        else
            NavigationManager.NavigateTo("mydevice", false, true);

        if (IntentHolder.FileUrl is not null)
        {
            var encodedArtifactPath = WebUtility.UrlEncode(IntentHolder.FileUrl);

            await ViewFileService.ViewFileAsync(IntentHolder.FileUrl, $"/MyDevice?encodedArtifactPath={encodedArtifactPath}");
        }
    }
}