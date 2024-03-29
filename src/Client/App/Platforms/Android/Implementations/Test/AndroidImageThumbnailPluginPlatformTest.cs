﻿using Android.Graphics;
using Functionland.FxFiles.Client.Shared.TestInfra.Implementations.ThumbnailPlugin;

namespace Functionland.FxFiles.Client.App.Platforms.Android.Implementations.Test;

public class AndroidImageThumbnailPluginPlatformTest<TFileService> : ImageThumbnailPlatformTest<TFileService>
    where TFileService : IFileService
{
    TFileService FileService { get; set; }

    public AndroidImageThumbnailPluginPlatformTest(IArtifactThumbnailService<TFileService> artifactThumbnailService,
                                                   TFileService fileService) : base(artifactThumbnailService, fileService)
    {
        FileService = fileService;
    }

    public override string Title => $"AndroidImageThumbnailPluginPlatfromTest {typeof(TFileService).Name}";

    public override string Description => "Test for creatign image thumbnail on Android.";

    protected override string OnGetRootPath() => "/storage/emulated/0/";

    protected override (int width, int height) GetArtifactWidthAndHeight(string imagePath)
    {
        BitmapFactory.Options options = new()
        {
            InJustDecodeBounds = true
        };
        _ = BitmapFactory.DecodeFile(imagePath, options);

        return (options.OutWidth, options.OutHeight);
    }
}
