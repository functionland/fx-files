using Functionland.FxFiles.Client.Shared.Enums;
using Functionland.FxFiles.Client.Shared.Services.Contracts;
using Functionland.FxFiles.Client.Shared.TestInfra.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Functionland.FxFiles.Client.App.Platforms.Windows.Implementations.Test
{
    public class WindowsPdfThumbnailPluginPlatformTest<TFileService> : ArtifactThumbnailPlatformTest<TFileService>
    where TFileService : IFileService
    {
        TFileService FileService { get; set; }

        public WindowsPdfThumbnailPluginPlatformTest(IArtifactThumbnailService<TFileService> artifactThumbnailService, TFileService fileService) 
            : base(artifactThumbnailService, fileService)
        {
            FileService = fileService;
        }

        public override string Title => $"WindowsPdfThumbnailPluginPlatformTest {typeof(TFileService).Name}";

        public override string Description => "Test for create artifact thumbnail on windows";

        protected override string OnGetRootPath() => "c:\\";

        protected override async Task OnRunThumbnailPluginAsync(string testRootPath)
        {
            using FileStream fs = File.Open(@"", FileMode.Open);

            var createdImage = await FileService.CreateFileAsync($@"{testRootPath}\1.jpg", fs);

            var thumbnailPath = await ArtifactThumbnailService.GetOrCreateThumbnailAsync(createdImage, ThumbnailScale.Medium);

            Assert.IsNotNull(thumbnailPath, "Image thumbnail created");

            var imageThumbnailArtifact = await FileService.GetArtifactAsync(thumbnailPath);

            Assert.IsNotNull(imageThumbnailArtifact, "Image thumbnail artifact founded!");
        }
    }
}
