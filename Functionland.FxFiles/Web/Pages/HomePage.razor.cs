using Functionland.FxFiles.App.Components.Common;
using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Functionland.FxFiles.App.Pages;

public partial class HomePage
{
    private string internalfilePath2;

    [AutoInject] public IFileService FileService { get; set; }
    public ListItemConfig Config { get; set; } = new ListItemConfig(ViewMode.grid, "this is title of item.txt", "bla bla", true, "text-file");
    private void NavigateToTestExplorerComponent()
    {
        NavigationManager.NavigateTo("/TestExplorer");
    }
}

