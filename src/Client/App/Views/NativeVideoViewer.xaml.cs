#if ANDROID
using Android.App;
using Android.Util;
#endif

using CommunityToolkit.Maui.MediaElement;

namespace Functionland.FxFiles.Client.App.Views;

public partial class NativeVideoViewer : ContentPage
{
    public NativeVideoViewer(string path)
    {
        InitializeComponent();
        mediaElement.Play();
        mediaElement.Source = MediaSource.FromFile(path);
    }

    private async void close_Clicked(object sender, EventArgs e)
    {
        try
        {
            mediaElement.Stop();
            mediaElement.Source = null;
            await Navigation.PopAsync();
        }
        catch (Exception ex)
        {

        }
    }

    public void Minimize(object sender, EventArgs e)
    {

        // Hide the controls in picture-in-picture mode.
        //     MainActivity.mainActivity.Minimize();
#if ANDROID
        var aspectRatio = new Rational(300, 300);
        PictureInPictureParams.Builder pictureInPictureParamsBuilder = new PictureInPictureParams.Builder();
        var piparam = pictureInPictureParamsBuilder.SetAspectRatio(aspectRatio).Build();
        Platform.CurrentActivity.EnterPictureInPictureMode(pictureInPictureParamsBuilder.Build());
#endif

    }
}