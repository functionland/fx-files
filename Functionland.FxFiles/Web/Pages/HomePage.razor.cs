using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

using Functionland.FxFiles.App.Components.Common;

namespace Functionland.FxFiles.App.Pages;

public partial class HomePage
{
    private string internalfilePath2;

    [AutoInject] public IFileService FileService { get; set; }
    private void NavigateToTestExplorerComponent()
    {
        NavigationManager.NavigateTo("/TestExplorer");
    }

    public bool IsBottomSheetClose { get; set; } = true;

    public void SyncCloseState(bool isClose)
    {
        IsBottomSheetClose = isClose;
    }

    public void OpenSheet()
    {
        IsBottomSheetClose = false;
    }
}

