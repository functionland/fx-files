using Functionland.FxFiles.Client.Shared.Enums;
using Functionland.FxFiles.Client.Shared.Services.Contracts;
using Functionland.FxFiles.Client.Shared.TestInfra.Implementations;

namespace Functionland.FxFiles.Client.App.Platforms.Windows.Implementations.Test;

public class WindowsImageThumbnailPluginPlatformTest<TFileService> : ArtifactThumbnailPlatformTest<TFileService>
    where TFileService : IFileService
{
    TFileService FileService { get; set; }

    public WindowsImageThumbnailPluginPlatformTest(IArtifactThumbnailService<TFileService> artifactThumbnailService,
                                                   TFileService fileService) : base(artifactThumbnailService, fileService)
    {
        FileService = fileService;
    }

    public override string Title => $"WindowsImageThumbnailPluginPlatformTest {typeof(TFileService).Name}";

    public override string Description => "Test for create artifact thumbnail on windows";

    protected override string OnGetRootPath() => "c:\\";

    protected override async Task OnRunThumbnailPluginAsync(string testRootPath)
    {
        using FileStream fs = File.Open(Path.Combine(GetSampleFileLocalPath(), "fake-pic.jpg"), FileMode.Open);

        var createdImage = await FileService.CreateFileAsync($@"{testRootPath}\1.jpg", fs);

        var thumbnailPath = await ArtifactThumbnailService.GetOrCreateThumbnailAsync(createdImage, ThumbnailScale.Medium);

        Assert.IsNotNull(thumbnailPath, "Image thumbnail created");

        var imageThumbnailArtifact = await FileService.GetArtifactAsync(thumbnailPath);

        Assert.IsNotNull(imageThumbnailArtifact, "Image thumbnail artifact founded!");
    }
}
