using Android.Content;
using Android.OS.Storage;
using Functionland.FxFiles.Client.Shared.Exceptions;
using Functionland.FxFiles.Client.Shared.Resources;
using Functionland.FxFiles.Client.Shared.TestInfra.Implementations;

namespace Functionland.FxFiles.Client.App.Platforms.Android.Implementations.Test;

public class AndroidExternalArtifactThumbnailPlatformTest<TFileService> : ArtifactThumbnailPlatformTest<TFileService>
    where TFileService : IFileService
{
    public IStringLocalizer<AppStrings> StringLocalizer { get; set; }

    public override string Title => $"AndroidExternalArtifactThumbnailPlatformTest {typeof(TFileService).Name}";

    public override string Description => "Test for create artifact thumbnail on android in external storage";

    public AndroidExternalArtifactThumbnailPlatformTest(IArtifactThumbnailService<TFileService> artifactThumbnailService,
                                                           TFileService fileService,
                                                           IStringLocalizer<AppStrings> stringLocalizer) : base(artifactThumbnailService, fileService)
    {
        StringLocalizer = stringLocalizer;
    }



    protected override string OnGetRootPath()
    {
        var storageManager = MauiApplication.Current.GetSystemService(Context.StorageService) as StorageManager;
        if (storageManager is null)
        {
            throw new UnableAccessToStorageException(StringLocalizer.GetString(AppStrings.UnableToLoadStorageManager));
        }

        var externalRootPath = storageManager.StorageVolumes.Where(s => s.IsPrimary == false).Select(d => d.Directory?.Path).FirstOrDefault();

        if (externalRootPath == null)
        {
            throw new ArtifactPathNullException(StringLocalizer.GetString(AppStrings.ArtifactPathIsNull, "external drive"));
        }
        else return externalRootPath;
    }
}
