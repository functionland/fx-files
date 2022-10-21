﻿using System.Net;

namespace Functionland.FxFiles.Client.Shared.Pages.FileViewer;

public partial class ZipFileViewerPage : IFileViewerPage
{
    [AutoInject] public ILocalDeviceFileService LocalDeviceFileService { get; set; } = default!;
    [AutoInject] public IFulaFileService FulaFileService { get; set; } = default!;

    [Parameter] public string EncodedFullPath { get; set; } = default!;
    [Parameter] public string EncodedReturnUrl { get; set; } = default!;
    [Parameter] public string FileServiceProvider { get; set; } = default!;

    private string ZipPath { get; set; } = string.Empty;
    private IFileService FileService { get; set; } = default!;


    protected override async Task OnInitAsync()
    {
        if (FileServiceProvider is "FulaFileService")
        {
            FileService = FulaFileService;
        }
        else
        {
            FileService = LocalDeviceFileService;
        }
        var decodedPath = WebUtility.UrlDecode(EncodedFullPath);

        ZipPath = decodedPath;


        await base.OnInitAsync();
    }

    private void Close()
    {
        var decodedReturnUrl = WebUtility.UrlDecode(EncodedReturnUrl);
        NavigationManager.NavigateTo(decodedReturnUrl);
    }
}
