using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

using Functionland.FxFiles.App.Components.Common;
using Functionland.FxFiles.App.Components.DesignSystem;
using Functionland.FxFiles.App.Components.Modal;

namespace Functionland.FxFiles.App.Pages;

public partial class HomePage
{
    public bool IsBottomSheetClose { get; set; } = false;

    private ToastModal? _toast { get; set; } = default!;

    public void SyncCloseState(bool isClose)
    {
        IsBottomSheetClose = isClose;
    }

    public void OpenSheet()
    {
        _toast.Show("salam", "salam", FxToastType.Info);
    }
}

