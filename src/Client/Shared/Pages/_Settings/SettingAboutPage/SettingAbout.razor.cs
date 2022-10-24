using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Functionland.FxFiles.Client.Shared.Pages
{
    public partial class SettingAbout
    {
        private void HandleToolbarBack()
        {
            NavigationManager.NavigateTo("settings", false, true);
        }
    }
}
