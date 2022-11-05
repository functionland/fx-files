
using Functionland.FxFiles.Client.App.Platforms.Android.Contracts;
using Functionland.FxFiles.Client.Shared.Enums;
using Functionland.FxFiles.Client.Shared.Exceptions;
using Functionland.FxFiles.Client.Shared.Resources;

namespace Functionland.FxFiles.Client.App.Platforms.Android.Implementations;

public partial class Android11andAboveFileService : AndroidFileService
{
    protected override async Task GetWritePermission(string path = null)
    {
        if (!await PermissionUtils.CheckWriteStoragePermissionAsync(path))
        {
            await PermissionUtils.RequestStoragePermission();

            var StoragePermissionResult = await PermissionUtils.GetPermissionTask!.Task;
            if (!StoragePermissionResult)
            {
                throw new UnableAccessToStorageException(StringLocalizer.GetString(AppStrings.UnableToAccessToStorage));
            }
        }
    }

    protected override async Task GetWritePermission(IEnumerable<string> paths = null)
    {
        if (paths == null || !paths.Any())
        {
            await GetWritePermission(String.Empty);
        }

        foreach (var path in paths)
        {
            await GetWritePermission(path);
        }
    }

    protected override async Task GetReadPermission(string path = null)
    {
        await GetWritePermission(path);
    }

    protected override async Task GetReadPermission(IEnumerable<string> paths = null)
    {
        if (paths == null || !paths.Any())
        {
            await GetWritePermission(String.Empty);
        }

        foreach (var path in paths)
        {
            await GetWritePermission(path);
        }
    }
}
