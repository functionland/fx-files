using Functionland.FxFiles.Client.Shared.Services.Implementations;
using System.Net;

namespace Functionland.FxFiles.Client.Shared.Pages.FileViewer;

public partial class TextFileViewerPage : IFileViewerPage
{
    [AutoInject] public ILocalDeviceFileService LocalDeviceFileService { get; set; }=default!;
    [AutoInject] public IFulaFileService FulaFileService { get; set; } = default!;

    [Parameter] public string EncodedFullPath { get; set; } = default!;
    [Parameter] public string FileServiceProvider { get; set; } = default!;

    private string Text { get; set; } = string.Empty;
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

        using var stream = await FileService.GetFileContentAsync(decodedPath);
        using var sr = new StreamReader(stream);

        Text = sr.ReadToEnd();

        await base.OnInitAsync();
    }
}
