using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

using Functionland.FxFiles.App.Components.Common;
using Functionland.FxFiles.App.Components.DesignSystem;
using Functionland.FxFiles.App.Components.Modal;

namespace Functionland.FxFiles.App.Pages;

public partial class HomePage
{
    [AutoInject]
    private IFileService _fileService = default!;

    public bool IsBottomSheetClose { get; set; } = false;

    public void SyncCloseState(bool isClose)
    {
        IsBottomSheetClose = isClose;
    }
}

