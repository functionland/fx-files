using Android.Content;
using Android.OS.Storage;
using android = Android;

namespace Functionland.FxFiles.Client.App.Platforms.Android.PermissionsUtility;

public class Android11andAbovePermissionUtils : PermissionUtils
{
    public override int StoragePermissionRequestCode { get; set; } = 2296;
}

