using Android.Content;
using Android.OS.Storage;
using Functionland.FxFiles.Client.Shared.Exceptions;
using Functionland.FxFiles.Client.Shared.Resources;
using Functionland.FxFiles.Client.Shared.TestInfra.Implementations;

namespace Functionland.FxFiles.Client.App.Platforms.Android.Implementations.Test;

public partial class ExternalAndroidFileServicePlatformTest : FileServicePlatformTest
{
    public ILocalDeviceFileService FileService { get; set; }
    public IStringLocalizer<AppStrings> StringLocalizer { get; set; }

    public ExternalAndroidFileServicePlatformTest(ILocalDeviceFileService fileService, IStringLocalizer<AppStrings> stringLocalizer)
    {
        FileService = fileService;
        StringLocalizer = stringLocalizer;
    }
    public override string Title => "External AndroidFileService Test";

    public override string Description => "Tests the common features of this FileService external storage of Android.";

    protected override IFileService OnGetFileService()
    {
        return FileService;
    }

    protected override string OnGetTestsRootPath()
    {
        var storageManager = MauiApplication.Current.GetSystemService(Context.StorageService) as StorageManager;
        if (storageManager is null)
        {
            throw new UnableAccessToStorageException(StringLocalizer.GetString(AppStrings.UnableToLoadStorageManager));
        }

        var externalRootPath = storageManager.StorageVolumes.Where(s => s.IsPrimary == false).Select(d => $@"/storage/{d.Uuid}").FirstOrDefault();

        if (externalRootPath == null)
        {
            throw new ArtifactPathNullException(StringLocalizer.GetString(AppStrings.ArtifactPathIsNull, "external drive"));
        }
        else return externalRootPath;

    }
}
