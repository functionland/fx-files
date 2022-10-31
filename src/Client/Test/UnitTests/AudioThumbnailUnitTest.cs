using Functionland.FxFiles.Client.Shared.Enums;
using Functionland.FxFiles.Client.Shared.Models;
using Functionland.FxFiles.Client.Shared.Services.Contracts;
using Functionland.FxFiles.Client.Shared.Services.Implementations.Db;
using Functionland.FxFiles.Client.Shared.TestInfra.Implementations;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Security.Cryptography.X509Certificates;

// Include the System Drawing classes
using System.Drawing.Imaging;
using System.Drawing;
using NAudio.WaveFormRenderer;
using NAudio.Wave;
using static Dapper.SqlMapper;

namespace Functionland.FxFiles.Client.Test.UnitTests
{
    [TestClass]
    public class AudioThumbnailUnitTest : TestBase
    {

        public TestContext TestContext { get; set; }

        [TestMethod]
        public async Task AddPinDbServiceUnitTest_MustWork()
        {
            // 1. Configure Providers
            MaxPeakProvider maxPeakProvider = new MaxPeakProvider();
            RmsPeakProvider rmsPeakProvider = new RmsPeakProvider(200); // e.g. 200
            SamplingPeakProvider samplingPeakProvider = new SamplingPeakProvider(200); // e.g. 200
            AveragePeakProvider averagePeakProvider = new AveragePeakProvider(4); // e.g. 4

            // 2. Configure the style of the audio wave image
            StandardWaveFormRendererSettings myRendererSettings = new StandardWaveFormRendererSettings();
            myRendererSettings.Width = 1080;
            myRendererSettings.TopHeight = 64;
            myRendererSettings.BottomHeight = 64;
            myRendererSettings.TopPeakPen = new Pen(Color.FromArgb(204, 229, 255));
            myRendererSettings.BottomPeakPen = new Pen(Color.FromArgb(204, 204, 255));
            myRendererSettings.BackgroundColor = Color.White;

            // 3. Define the audio file from which the audio wave will be created and define the providers and settings
            WaveFormRenderer renderer = new WaveFormRenderer();
            String audioFilePath = @"D:\ghesse bita.mp3";
            Image image = null;
            using (var waveStream = new AudioFileReader(audioFilePath))
            {
                image = renderer.Render(waveStream, averagePeakProvider, myRendererSettings);
            }
            // 4. Store the image 
           //// image.Save(@"D:\", ImageFormat.Png);
           // var i2 = new Bitmap(image);
           // i2.Save(@"D:\", ImageFormat.Jpeg);
            var memoryStream = new MemoryStream();

             image.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Jpeg);
            // write stream
            using (var fileStream = File.Create(@"D:\thumb2.png"))
            {
                memoryStream.Seek(0, SeekOrigin.Begin);
                memoryStream.CopyTo(fileStream);
            }
            // Or jpeg, however PNG is recommended if your audio wave needs transparency
            // image.Save(@"C:\Users\sdkca\Desktop\myfile.jpg", ImageFormat.Jpeg);
        }


    }
}
