using Android.App;
using Android.Content;
using Android.OS.Storage;
using Functionland.FxFiles.Client.Shared.Exceptions;
using Functionland.FxFiles.Client.Shared.Resources;
using Microsoft.Extensions.Localization;
using Microsoft.Maui.Controls.Compatibility.Platform.Android;
using android = Android;
using Platform = Microsoft.Maui.ApplicationModel.Platform;

namespace Functionland.FxFiles.Client.App.Platforms.Android.PermissionsUtility;

public partial class Android10PermissionUtils : PermissionUtils
{
    [AutoInject] public IStringLocalizer<AppStrings> StringLocalizer { get; set; } = default!;

    public override int StoragePermissionRequestCode { get; set; } = 2020;

    public override async Task RequestStoragePermission(string filepath = null)
    {
        GetPermissionTask = new TaskCompletionSource<bool>();

        var isInternal = string.IsNullOrWhiteSpace(filepath) || IsInternalFilePath(filepath);

        if (isInternal)
        {
            await Permissions.RequestAsync<Android10StoragePermission>();
            GetPermissionTask.SetResult(await CheckStoragePermissionAsync());
        }
        else
        {
            var storageManager = MauiApplication.Current.GetSystemService(Context.StorageService) as StorageManager;
            var volume = storageManager.StorageVolumes.ToList().FirstOrDefault(r => !r.IsPrimary);
            var intent = volume.CreateOpenDocumentTreeIntent();
            Platform.CurrentActivity?.StartActivityForResult(intent, StoragePermissionRequestCode);
        }
    }

    public override async Task<bool> CheckStoragePermissionAsync(string filepath = null)
    {
        var isInternal = string.IsNullOrWhiteSpace(filepath) || IsInternalFilePath(filepath);

        if (isInternal)
        {
            return await Permissions.CheckStatusAsync<Android10StoragePermission>() == PermissionStatus.Granted;
        }
        else
        {
            var permission = MauiApplication.Context?.ContentResolver?.PersistedUriPermissions.ToList();
            var permissionList = new List<(UriPermission Permission, string SepmentPath)>();
            foreach (var p in permission)
            { 
                var segmentpath = System.IO.Path.Combine(p.Uri.PathSegments.Skip(1).Select(r => r.TrimEnd(':')).ToArray());
                segmentpath = segmentpath.Replace(":", Java.IO.File.Separator);
                permissionList.Add((Permission: p, SepmentPath: segmentpath));
            };

            var check = permissionList.Any(r => r.Permission.IsWritePermission && filepath.StartsWith($"/storage/{r.SepmentPath}"));

            return check;

        }
    }

    public override async Task OnPermissionResult(Result resultCode, Intent? data)
    {
        if (resultCode is Result.Ok)
        {
            var treeUri = data;
            var takeFlags = data!.Flags & (ActivityFlags.GrantReadUriPermission | ActivityFlags.GrantWriteUriPermission);
            MauiApplication.Context?.ContentResolver?.TakePersistableUriPermission(treeUri.Data, takeFlags);
            GetPermissionTask?.SetResult(true);
        }
        else
        {
            GetPermissionTask?.SetResult(false);
        }
    }

    private bool IsInternalFilePath(string filepath)
    {
        var storage = android.OS.Environment.ExternalStorageDirectory;
        return filepath.StartsWith(storage.AbsolutePath);
    }
}

