using Functionland.FxFiles.Client.Shared.TestInfra.Implementations.ThumbnailPlugin;
using Image = System.Drawing.Image;

namespace Functionland.FxFiles.Client.App.Platforms.Windows.Implementations.Test;

public class WindowsAudioThumbnailPluginPlatformTest<TFileService> : ImageThumbnailPlatformTest<TFileService>
    where TFileService : IFileService
{
    TFileService FileService { get; set; }

    public WindowsAudioThumbnailPluginPlatformTest(IArtifactThumbnailService<TFileService> artifactThumbnailService,
                                                   TFileService fileService) : base(artifactThumbnailService, fileService)
    {
        FileService = fileService;
    }

    public override string Title => $"WindowsAudioThumbnailPluginPlatformTest {typeof(TFileService).Name}";

    public override string Description => "Test for create image thumbnail on windows";

    protected override string OnGetRootPath() => "c:\\";

    protected override (int width, int height) GetArtifactWidthAndHeight(string imagePath)
    {
        var image = Image.FromFile(imagePath);
        return (image.Width, image.Height);
    }
}
