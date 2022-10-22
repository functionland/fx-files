using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Permission = Android.Manifest.Permission;

namespace Functionland.FxFiles.Client.App.Platforms.Android.PermissionsUtility
{
    public class FunctionlandPermission : Permissions.BasePlatformPermission
    {
        public override (string androidPermission, bool isRuntime)[] RequiredPermissions => new List<(string androidPermission, bool isRuntime)> {
                (Permission.Internet, true),
                (Permission.AccessNetworkState, true),
                (Permission.WriteExternalStorage, true),
                (Permission.ReadExternalStorage, true)

        }.ToArray();

    }
}
