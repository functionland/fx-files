using Functionland.FxFiles.App.Components.Common;
using Functionland.FxFiles.App.Components.Modal;

namespace Functionland.FxFiles.App.Pages
{
    public partial class MyDevice
    {
        private ArtifactSelectionModal _artifactSelectionModalRef = default!;
        private FxListView _fxListViewRef = default!;

        [AutoInject]
        private IFileService _fileService { get; set; } = default!;
        public List<FsArtifact> Artifacts { get; set; } = new List<FsArtifact>();
        private List<FsArtifact> _selectedArtifacts = new List<FsArtifact>();
        private string _message = string.Empty;

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
            var artifacts = new List<FsArtifact>();

            await foreach (var item in allFiles)
            {
                artifacts.Add(item);
            }

            Artifacts = artifacts;
        }
        #endregion

        private async Task Copy()
        {
            try
            {
                var result = await _artifactSelectionModalRef.ShowAsync();
                if (result.ResultType == ArtifactSelectionResultType.Ok)
                {
                    await Task.Delay(2000);
                    await _fileService.CopyArtifactsAsync(_fxListViewRef.SelectedListItems.ToArray(), result.SelectedArtifacts.Single().FullPath);
                    _message = $"Copy successful {_fxListViewRef.SelectedListItems.Count} files";
                }

                else
                {
                    await Task.Delay(2000);
                    _message = "Copy cancelled";
                }
            }
            catch (Exception e)
            {
                _message = e.Message;
            }
        }
    }
}
