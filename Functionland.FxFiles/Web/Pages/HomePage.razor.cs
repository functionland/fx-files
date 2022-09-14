using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Functionland.FxFiles.App.Pages;

public partial class HomePage
{
    private string internalfilePath2;

    [AutoInject] public IFileService FileService { get; set; }
    private void NavigateToTestExplorerComponent()
    {
        NavigationManager.NavigateTo("/TestExplorer");
    }
}

