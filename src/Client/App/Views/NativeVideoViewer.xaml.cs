using CommunityToolkit.Maui.MediaElement;
using Microsoft.AspNetCore.Components;

namespace Functionland.FxFiles.Client.App.Views;

public partial class NativeVideoViewer : ContentPage
{
    protected IExceptionHandler ExceptionHandler { get; set; } = default!;

    private bool IsInPictureInPicture { get; set; }

    private MediaElementState _currentMediaState = MediaElementState.Playing;

    private EventCallback OnBack { get; set; }

    public NativeVideoViewer(string path, EventCallback onBack)
    {
        InitializeComponent();

        if (path is not null)
        {
            media.Source = MediaSource.FromFile(path);
            playButton.Source = ImageSource.FromFile("pause.png");
            media.Play();
        }
        OnBack = onBack;
    }

    protected override void OnSizeAllocated(double width, double height)
    {
        if (!IsInPictureInPicture)
        {
            media.WidthRequest = width;
            media.HeightRequest = height;
        }

        base.OnSizeAllocated(width, height);
    }

    protected override bool OnBackButtonPressed()
    {
        HandleBack();
        return true;
    }

    private void Back_Clicked(object sender, EventArgs e)
    {
        HandleBack();
    }

    private async void HandleBack()
    {
        try
        {
            media.Stop();
            media.Source = null;
            await Navigation.PopAsync();
            await OnBack.InvokeAsync();
        }
        catch (Exception ex)
        {
            ExceptionHandler.Handle(ex);
        }
    }

    public void Minimize(object sender, EventArgs e)
    {
        header.IsVisible = false;
        mediaControls.IsVisible = false;

#if ANDROID
        var aspectRatio = new Android.Util.Rational(700, 400);
        Android.App.PictureInPictureParams.Builder pictureInPictureParamsBuilder = new Android.App.PictureInPictureParams.Builder();
        var piparam = pictureInPictureParamsBuilder.SetAspectRatio(aspectRatio).Build();
        Platform.CurrentActivity.EnterPictureInPictureMode(pictureInPictureParamsBuilder.Build());
        IsInPictureInPicture = Platform.CurrentActivity.IsInPictureInPictureMode;
#endif
    }

    private void media_Tapped(object sender, EventArgs e)
    {
        header.IsVisible = !header.IsVisible;
        mediaControls.IsVisible = !mediaControls.IsVisible;
    }

    private void media_MediaEnded(object sender, EventArgs e)
    {
        _currentMediaState = MediaElementState.Stopped;
        playButton.Source = ImageSource.FromFile("play.png");
    }

    private void PausePlay_Clicked(object sender, EventArgs e)
    {
#if !ANDROID
        _currentMediaState = media.CurrentState;
#endif
        if (_currentMediaState == MediaElementState.Playing)
        {
            media.Pause();
            playButton.Source = ImageSource.FromFile("play.png");
            _currentMediaState = MediaElementState.Paused;
        }
        else if (_currentMediaState == MediaElementState.Paused || _currentMediaState == MediaElementState.Stopped)
        {
            media.Play();
            playButton.Source = ImageSource.FromFile("pause.png");
            _currentMediaState = MediaElementState.Playing;
        }
    }

    private void Backward_Clicked(object sender, EventArgs e)
    {
        media.Position = TimeSpan.FromSeconds(media.Position.TotalSeconds - 15);
    }

    private void Forward_Clicked(object sender, EventArgs e)
    {
        media.Position = TimeSpan.FromSeconds(media.Position.TotalSeconds + 15);
    }
}