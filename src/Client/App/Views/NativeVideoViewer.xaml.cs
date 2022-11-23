#if ANDROID
using Android.App;
using Android.Graphics;
using Android.Media;
using Android.Util;
#endif

using Functionland.FxFiles.Client.App.Controls;
using VideoSource = Functionland.FxFiles.Client.App.Controls.VideoSource;

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

    private readonly string? _filePath;

    public NativeVideoViewer(string path)
    {
        InitializeComponent();

        if (path is not null)
        {
            _filePath = path;
            media.Source = VideoSource.FromFile(_filePath);
        }
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
            await Navigation.PopAsync();
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

    private void PausePlay_Clicked(object sender, EventArgs e)
    {
        if (media.Status == VideoStatus.Playing)
        {
            media.Pause();
        }
        else if (media.Status == VideoStatus.Paused || media.Status == VideoStatus.NotReady)
        {
            media.Play();
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