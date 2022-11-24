using Functionland.FxFiles.Client.Shared.Enums;
using Functionland.FxFiles.Client.Shared.Utils;
using NAudio.Wave;
using NAudio.WaveFormRenderer;
using System.Drawing;
using Color = System.Drawing.Color;
using Image = System.Drawing.Image;

namespace Functionland.FxFiles.Client.App.Platforms.Android.Implementations;

internal class WindowsAudioThumbnailPlugin : AudioThumbnailPlugin
{
    protected override async Task<Stream> OnCreateThumbnailAsync(Stream? stream, string? filePath, ThumbnailScale thumbnailScale, CancellationToken? cancellationToken = null)
    {
        if (filePath is null)
            throw new InvalidOperationException("FilePath should be provided for this plugin.");

        var (width, height) = ImageUtils.GetHeightAndWidthFromThumbnailScale(thumbnailScale);

        AveragePeakProvider averagePeakProvider = new AveragePeakProvider(4);

        StandardWaveFormRendererSettings fxRendererSettings = new StandardWaveFormRendererSettings();
        fxRendererSettings.Width = 1080;
        fxRendererSettings.TopHeight = 64;
        fxRendererSettings.BottomHeight = 64;
        fxRendererSettings.TopPeakPen = new Pen(Color.FromArgb(4, 155, 143));
        fxRendererSettings.BottomPeakPen = new Pen(Color.FromArgb(52, 58, 64));
        fxRendererSettings.BackgroundColor = Color.FromArgb(233, 236, 239);

        WaveFormRenderer renderer = new WaveFormRenderer();

        Image thumb;
        using (var waveStream = new AudioFileReader(filePath))
        {
            Image image = renderer.Render(waveStream, averagePeakProvider, fxRendererSettings);
            thumb = image.GetThumbnailImage(width, height, () => false, IntPtr.Zero);
        }

        var memoryStream = new MemoryStream();
        thumb.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Jpeg);
        return memoryStream;
    }
}