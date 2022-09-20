using Functionland.FxFiles.App.Components.Common;
using Functionland.FxFiles.App.Components.Modal;

namespace Functionland.FxFiles.App.Pages
{
    public partial class MyDevice
    {
        private ArtifactSelectionModal _artifactSelectionModalRef = default!;

        [AutoInject]
        private IFileService _fileService { get; set; } = default!;
        public List<ListItemConfig> ListItems { get; set; } = new List<ListItemConfig>();

        public List<FileCardConfig> PinnedCards = new List<FileCardConfig>
        {
            new FileCardConfig(true, true, true, "Cs intenrship", ".txt", "date", "file size"),
            new FileCardConfig(true, true, true, "Fx Land", ".mp3", "date", "file size"),
            new FileCardConfig(true, true, true, "doument", ".pdf", "date", "file size"),
            new FileCardConfig(true, true, true, "Cs intenrship", ".txt", "date", "file size")
        };

        protected override async Task OnInitAsync()
        {
            await GetAllFilesAsync();
        }

        #region File service methods

        public async Task GetAllFilesAsync()
        {
            var allFiles = _fileService.GetArtifactsAsync();
            //while (await allFiles.MoveNextAsync())
            //{
            //    var item = allFiles.Current;
            //    //TODO:impediment file type with icon later
            //    if (item.ArtifactType == FsArtifactType.Folder)
            //    {
            //        ListItems.Add(new ListItemConfig(item.Name, $"Modified {item.LastModifiedDateTime.Date.ToShortDateString()}", item.IsPinned, "folder"));
            //    }
            //    else
            //    {
            //        ListItems.Add(new ListItemConfig(item.Name, $"Modified {item.LastModifiedDateTime.Date.ToShortDateString()} | {item.Size}", item.IsPinned, "text-file"));
            //    }
            //}

            await foreach (var item in allFiles)
            {
                switch (item.ArtifactType)
                {
                    case FsArtifactType.File:
                        ListItems.Add(new ListItemConfig(item.Name, $"Modified {item.LastModifiedDateTime.Date.ToShortDateString()} | {item.Size}", item.IsPinned, "text-file"));
                        break;
                    case FsArtifactType.Folder:
                        ListItems.Add(new ListItemConfig(item.Name, $"Modified {item.LastModifiedDateTime.Date.ToShortDateString()}", item.IsPinned, "folder"));
                        break;
                    case FsArtifactType.Drive:
                        ListItems.Add(new ListItemConfig(item.Name, $"Modified {item.LastModifiedDateTime.Date.ToShortDateString()}", item.IsPinned, "drive"));
                        break;
                    default:
                        break;
                }
            }
        }
        #endregion

        private async Task Copy()
        {
            var result = await _artifactSelectionModalRef.ShowAsync();

            if (result.ResultType == ArtifactSelectionResultType.Ok)
            {
                await Task.Delay(2000);
                var result2 = await _artifactSelectionModalRef.ShowAsync();
            }
            else
            {

            }
        }
    }
}
