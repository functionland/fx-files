using Android.Graphics;
using Functionland.FxFiles.Client.Shared.TestInfra.Implementations.ThumbnailPlugin;

namespace Functionland.FxFiles.Client.App.Platforms.Android.Implementations.Test;

public class AndroidAudioThumbnailPluginPlatformTest<TFileService> : AudioThumbnailPlatformTest<TFileService>
    where TFileService : IFileService
{
    TFileService FileService { get; set; }

    public AndroidAudioThumbnailPluginPlatformTest(IArtifactThumbnailService<TFileService> artifactThumbnailService,
                                                   TFileService fileService) : base(artifactThumbnailService, fileService)
    {
        FileService = fileService;
    }

    public override string Title => $"AndroidAudioThumbnailPluginPlatformTest {typeof(TFileService).Name}";

    public override string Description => "Test for creatign audio thumbnail on Android.";

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
