using Functionland.FxFiles.App.Components.Common;
using System.Runtime.CompilerServices;

namespace Functionland.FxFiles.App.Pages;

public partial class HomePage
{
    public ListItemConfig Config { get; set; } = new ListItemConfig(ViewMode.grid, "this is title of item.txt", "bla bla", true, "text-file");
    private void NavigateToTestExplorerComponent()
    {
        NavigationManager.NavigateTo("/TestExplorer");
    }
}

