#if ANDROID
using Android.App;
using Android.Util;
#endif

using CommunityToolkit.Maui.MediaElement;
using Microsoft.Maui.Devices;

namespace Functionland.FxFiles.Client.App.Views;

public partial class NativeVideoViewer : ContentPage
{
    public NativeVideoViewer(string path)
    {
        InitializeComponent();
        media.Play();
        media.Source = MediaSource.FromFile(path);
    }

    protected override void OnSizeAllocated(double width, double height)
    {
        media.WidthRequest = width;
        media.HeightRequest = height;

        base.OnSizeAllocated(width, height);
    }

    private async void back_Clicked(object sender, EventArgs e)
    {
        try
        {
            media.Stop();
            media.Source = null;
            await Navigation.PopAsync();
        }
        catch (Exception ex)
        {

        }
    }

    public void Minimize(object sender, EventArgs e)
    {
#if ANDROID
        var aspectRatio = new Rational(300, 300);
        PictureInPictureParams.Builder pictureInPictureParamsBuilder = new PictureInPictureParams.Builder();
        var piparam = pictureInPictureParamsBuilder.SetAspectRatio(aspectRatio).Build();
        Platform.CurrentActivity.EnterPictureInPictureMode(pictureInPictureParamsBuilder.Build());
#endif
    }

    private void media_Tapped(object sender, EventArgs e)
    {
        header.IsVisible = !header.IsVisible;
    }

    private void pin_Clicked(object sender, EventArgs e)
    {

    }

    private void overflow_Clicked(object sender, EventArgs e)
    {

    }
}