using CommunityToolkit.Maui.MediaElement;

namespace Functionland.FxFiles.Client.App.Views;

public partial class NativeVideoViewer : ContentPage
{
    protected IExceptionHandler ExceptionHandler { get; set; } = default!;

    private bool IsInPictureInPicture { get; set; }

    public NativeVideoViewer(string path)
    {
        InitializeComponent();
        media.Play();
        media.Source = MediaSource.FromFile(path);
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

    private void PausePlay_Clicked(object sender, EventArgs e)
    {
        if (media.CurrentState == MediaElementState.Playing)
            media.Pause();
        else if (media.CurrentState == MediaElementState.Paused || media.CurrentState == MediaElementState.Stopped)
            media.Play();
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