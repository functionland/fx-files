using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Microsoft.Maui;
using Microsoft.Maui.Platform;
using Functionland.FxFiles.Client.App.Views;

#if WINDOWS
using Windows.UI.ViewManagement;
#endif

namespace Functionland.FxFiles.Client.App.Implementations
{
    public class NativeNavigation : INativeNavigation
    {
        public async Task NavigateToVidoeViewer(string path)
        {
            var videoViewer = new NativeVideoViewer(path);
            await App.Current.MainPage.Navigation.PushAsync(videoViewer, true);
        }
    }
}
