using Functionland.FxFiles.Client.Shared.Components.Modal;
using Functionland.FxFiles.Client.Shared.Services.Common;
using Prism.Events;

namespace Functionland.FxFiles.Client.Shared.Pages;

public partial class HomePage
{
    [AutoInject] public ILocalDeviceFileService LocalDeviceFileService { get; set; } = default!;
    [AutoInject] public IEventAggregator EventAggregator { get; set; } = default!;
    [AutoInject] public IntentHolder IntentHolder { get; set; } = default!;

    protected override async Task OnInitAsync()
    {
        if (!string.IsNullOrWhiteSpace(IntentHolder.FileUrl))
        {
            NavigationManager.NavigateTo("mydevice", false, true);
            return;
        }
        _ = EventAggregator
                  .GetEvent<IntentReceiveEvent>()
                  .Subscribe(
                      HandleIntentReceiver,
                      ThreadOption.BackgroundThread, keepSubscriberReferenceAlive: true);

        await base.OnInitAsync();

        if (IsiOS)
            NavigationManager.NavigateTo("settings", false, true);
        else
            NavigationManager.NavigateTo("mydevice", false, true);
    }


    private void HandleIntentReceiver(IntentReceiveEvent obj)
    {
        NavigationManager.NavigateTo("mydevice", false, true);
    }
}