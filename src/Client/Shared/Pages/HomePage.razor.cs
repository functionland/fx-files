namespace Functionland.FxFiles.Client.Shared.Pages;

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

