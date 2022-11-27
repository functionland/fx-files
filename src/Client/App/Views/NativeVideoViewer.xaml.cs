#if ANDROID
using Android.App;
using Android.Graphics;
using Android.Media;
using Android.Util;
#endif

using CommunityToolkit.Maui.MediaElement;
using Microsoft.AspNetCore.Components;

namespace Functionland.FxFiles.Client.App.Views;

public partial class NativeVideoViewer : ContentPage
{
    protected IExceptionHandler ExceptionHandler { get; set; } = default!;

    private bool? _isInPictureInPicture = null;
    public bool? IsInPictureInPicture
    {
        set { if (_isInPictureInPicture == null) _isInPictureInPicture = value; }
        get { return _isInPictureInPicture; }
    }

    private EventCallback OnBack { get; set; }
    private MediaElementState CurrentMediaState { get; set; } = MediaElementState.Playing;
    public static NativeVideoViewer? Current { get; set; }

    private readonly string _filePath;

    public NativeVideoViewer(string path, EventCallback onBack)
    {
        InitializeComponent();
        Current = this;

        if (path is not null)
        {
            _filePath = path;
            media.Source = MediaSource.FromFile(_filePath);
            playButton.Source = ImageSource.FromFile("pause.png");
            media.Play();
        }

        OnBack = onBack;
    }

    protected override void OnSizeAllocated(double width, double height)
    {
        if (IsInPictureInPicture == false)
        {
            Device.StartTimer(TimeSpan.FromMilliseconds(1), () =>
            {
                media.WidthRequest = width;
                media.HeightRequest = height;

                return false;
            });

            base.OnSizeAllocated(width, height);
        }
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
            await OnBack.InvokeAsync();
            await Navigation.PopAsync();

            //MemoryLeak Killer
            media.Handler?.DisconnectHandler();
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
        MediaMetadataRetriever retriever = new MediaMetadataRetriever();
        retriever.SetDataSource(_filePath);
        Bitmap? bitmap = retriever.GetFrameAtTime(0);
        if (bitmap != null)
        {
            var aspectRatio = new Rational(bitmap.Width, bitmap.Height);
            PictureInPictureParams.Builder pictureInPictureParamsBuilder = new PictureInPictureParams.Builder();
            pictureInPictureParamsBuilder.SetAspectRatio(aspectRatio).Build();
            Platform.CurrentActivity.EnterPictureInPictureMode(pictureInPictureParamsBuilder.Build());
            _isInPictureInPicture = Platform.CurrentActivity.IsInPictureInPictureMode;
        }
#endif
    }

    private void media_Tapped(object sender, EventArgs e)
    {
        header.IsVisible = !header.IsVisible;
        mediaControls.IsVisible = !mediaControls.IsVisible;
    }

    private void Backward_Clicked(object sender, EventArgs e)
    {
        media.Position = TimeSpan.FromSeconds(media.Position.TotalSeconds - 15);
    }

    private void Forward_Clicked(object sender, EventArgs e)
    {
        media.Position = TimeSpan.FromSeconds(media.Position.TotalSeconds + 15);
    }

    private void Pause()
    {
        playButton.Source = ImageSource.FromFile("play.png");
        CurrentMediaState = MediaElementState.Paused;
        media.Pause();
    }

    private void Play()
    {
        playButton.Source = ImageSource.FromFile("pause.png");
        CurrentMediaState = MediaElementState.Playing;
        media.Play();
    }

    public void TogglePlay(MediaElementState state)
    {
        CurrentMediaState = media.CurrentState = state;

        if (CurrentMediaState == MediaElementState.Playing)
        {
            Play();
        }
        else if (CurrentMediaState == MediaElementState.Paused)
        {
            Pause();
        }
    }

    private void PausePlay_Clicked(object sender, EventArgs e)
    {
        //Workaround
#if !ANDROID
        CurrentMediaState = media.CurrentState;
#endif

        if (CurrentMediaState == MediaElementState.Playing)
        {
            Pause();
        }
        else if (CurrentMediaState == MediaElementState.Paused || CurrentMediaState == MediaElementState.Stopped)
        {
            Play();
        }

        //Workaround
#if ANDROID
        if (media.Duration.TotalSeconds <= media.Position.TotalSeconds || CurrentMediaState == MediaElementState.Stopped)
        {
            media.Position = TimeSpan.FromSeconds(0);
            Play();
        }
#endif
    }
}