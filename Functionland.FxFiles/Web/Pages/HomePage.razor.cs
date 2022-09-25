﻿using System;
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
        var result = await _detailModal.ShowAsync(artifacts, true);
        switch (result.ResultType)
        {
            case ArtifactDetailModalResultType.Download:
                _toast.Show("Download", "Download", FxToastType.Info);
                break;

            case ArtifactDetailModalResultType.Move:
                _toast.Show("Move", "Move", FxToastType.Info);
                break;

            case ArtifactDetailModalResultType.Pin:
                _toast.Show("Pin", "Pin", FxToastType.Info);
                break;

            case ArtifactDetailModalResultType.More:
                _toast.Show("More", "More", FxToastType.Info);
                break;
        }
    }
}

