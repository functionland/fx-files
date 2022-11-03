using Functionland.FxFiles.Client.Shared.Enums;
using Functionland.FxFiles.Client.Shared.Services.Implementations.Thumbnail.Plugins;
using Functionland.FxFiles.Client.Shared.Utils;
using NAudio.Wave;
using NAudio.WaveFormRenderer;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Color = System.Drawing.Color;
using Image = System.Drawing.Image;

namespace Functionland.FxFiles.Client.App.Platforms.Android.Implementations
{
    internal class WindowsAudioThumbnailPlugin : AudioThumbnailPlugin
    {
        protected override async Task<Stream> OnCreateThumbnailAsync(Stream? stream, string? filePath, ThumbnailScale thumbnailScale, CancellationToken? cancellationToken = null)
        {
            if (filePath is null)
                throw new InvalidOperationException("FilePath should be provided for this plugin.");

            var (width, height) = ImageUtils.GetHeightAndWidthFromThumbnailScale(thumbnailScale);
            
            AveragePeakProvider averagePeakProvider = new AveragePeakProvider(4);

            StandardWaveFormRendererSettings myRendererSettings = new StandardWaveFormRendererSettings();
            myRendererSettings.Width = 1080;
            myRendererSettings.TopHeight = 64;
            myRendererSettings.BottomHeight = 64;
            myRendererSettings.TopPeakPen = new Pen(Color.FromArgb(255, 139, 0));
            myRendererSettings.BottomPeakPen = new Pen(Color.FromArgb(158, 160, 165));
            myRendererSettings.BackgroundColor = Color.White;

            WaveFormRenderer renderer = new WaveFormRenderer();

            Image thumb;
            using (var waveStream = new AudioFileReader(filePath))
            {
                Image image =  renderer.Render(waveStream, averagePeakProvider, myRendererSettings);
                 thumb = image.GetThumbnailImage(width, height, () => false, IntPtr.Zero);
            }
            var memoryStream = new MemoryStream();

            thumb.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Jpeg);
            return memoryStream;

        }
    }
}
