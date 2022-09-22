using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

using Functionland.FxFiles.App.Components.Common;

namespace Functionland.FxFiles.App.Pages;

public partial class HomePage
{
    public bool IsBottomSheetClose { get; set; } = false;

    public void SyncCloseState(bool isClose)
    {
        IsBottomSheetClose = isClose;
    }

    public void OpenSheet()
    {
        IsBottomSheetClose = true;
    }
}

