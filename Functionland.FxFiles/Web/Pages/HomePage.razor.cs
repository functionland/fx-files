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

    private ToastModal? _toast { get; set; } = default!;

    private ArtifactDetailModal _detailModal { get; set; } = default!;

    public void SyncCloseState(bool isClose)
    {
        IsBottomSheetClose = isClose;
    }

    public void OpenSheet()
    {
        _toast.Show("salam", "salam", FxToastType.Info);
    }

    public async Task OpenDetail()
    {
        var artifacts = new List<FsArtifact>();
        var artifactsEnumerable = _fileService.GetArtifactsAsync();
        await foreach (var item in artifactsEnumerable)
        {
            artifacts.Add(item);
        }
        var result = await _detailModal.ShowAsync(artifacts);
    }
}

