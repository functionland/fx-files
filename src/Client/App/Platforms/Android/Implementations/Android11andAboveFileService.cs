
using Functionland.FxFiles.Client.App.Platforms.Android.Contracts;
using Functionland.FxFiles.Client.Shared.Enums;
using Functionland.FxFiles.Client.Shared.Exceptions;
using Functionland.FxFiles.Client.Shared.Resources;

namespace Functionland.FxFiles.Client.App.Platforms.Android.Implementations;

public partial class Android11andAboveFileService : AndroidFileService
{
    protected override async Task GetPermission(string path = null)
    {
        if (!await PermissionUtils.CheckStoragePermissionAsync())
        {
            await PermissionUtils.RequestStoragePermission();

            var StoragePermissionResult = await PermissionUtils.GetPermissionTask!.Task;
            if (!StoragePermissionResult)
            {
                throw new UnableAccessToStorageException(StringLocalizer.GetString(AppStrings.UnableToAccessToStorage));
            }
        }
    }

    protected override async Task GetPermission(IEnumerable<string> paths = null)
    {
        await GetPermission(String.Empty);
    }
}
