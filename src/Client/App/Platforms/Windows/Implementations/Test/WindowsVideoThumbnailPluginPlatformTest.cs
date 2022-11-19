﻿using Functionland.FxFiles.Client.Shared.TestInfra.Implementations.ThumbnailPlugin;
using Image = System.Drawing.Image;

namespace Functionland.FxFiles.Client.App.Platforms.Windows.Implementations.Test;

public class WindowsVideoThumbnailPluginPlatformTest<TFileService> : VideoThumbnailPlatformTest<TFileService>
    where TFileService : IFileService
{
    TFileService FileService { get; set; }

    public WindowsVideoThumbnailPluginPlatformTest(IArtifactThumbnailService<TFileService> artifactThumbnailService,
                                                   TFileService fileService) : base(artifactThumbnailService, fileService)
    {
        FileService = fileService;
    }

    public override string Title => $"WindowsVideoThumbnailPluginPlatformTest {typeof(TFileService).Name}";

    public override string Description => "Test for create video thumbnail on windows";

    protected override string OnGetRootPath() => "c:\\";

    protected override (int width, int height) GetArtifactWidthAndHeight(string imagePath)
    {
        var image = Image.FromFile(imagePath);
        return (image.Width, image.Height);
    }
}
