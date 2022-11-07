using Functionland.FxFiles.Client.App.Views;
using Microsoft.AspNetCore.Components;

namespace Functionland.FxFiles.Client.App.Implementations;

public class NativeNavigation : INativeNavigation
{
    public async Task NavigateToVidoeViewer(string path, EventCallback onBack)
    {
        if (Application.Current?.MainPage is null) return;

        var videoViewer = new NativeVideoViewer(path, onBack);
        await Application.Current.MainPage.Navigation.PushAsync(videoViewer, true);
    }
}
