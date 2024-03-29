﻿using Android.Graphics;
using Functionland.FxFiles.Client.Shared.TestInfra.Implementations.ThumbnailPlugin;

namespace Functionland.FxFiles.Client.App.Platforms.Android.Implementations.Test;

public class AndroidPdfThumbnailPluginPlatformTest<TFileService> : PdfThumbnailPlatformTest<TFileService>
    where TFileService : IFileService
{
    TFileService FileService { get; set; }

    public AndroidPdfThumbnailPluginPlatformTest(IArtifactThumbnailService<TFileService> artifactThumbnailService, TFileService fileService)
        : base(artifactThumbnailService, fileService)
    {
        FileService = fileService;
    }

    public override string Title => $"AndroidPdfThumbnailPluginPlatformTest {typeof(TFileService).Name}";

    public override string Description => "Test for creating pdf thumbnail on Android.";

    protected override string OnGetRootPath() => "/storage/emulated/0/";

    protected override (int width, int height) GetArtifactWidthAndHeight(string filePath)
    {
        BitmapFactory.Options options = new()
        {
            InJustDecodeBounds = true
        };
        _ = BitmapFactory.DecodeFile(filePath, options);

        return (options.OutWidth, options.OutHeight);
    }
}
