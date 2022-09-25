namespace Functionland.FxFiles.Client.Shared.Pages
{
    public partial class FxFolderView
    {

        [AutoInject]
        private IFileService _fileService { get; set; } = default!;
        public List<FsArtifact> Artifacts { get; set; } = new List<FsArtifact>();

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
    }
}
