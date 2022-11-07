namespace Functionland.FxFiles.Client.Shared.Components
{
    public partial class FxBreadcrumbs
    {
        [Parameter] public FsArtifact? Artifact { get; set; }
        [Parameter] public IFileService? FileService { get; set; }

        private string[] _breadCrumbsPath = Array.Empty<string>();

        protected override async Task OnInitAsync()
        {
            await base.OnInitAsync();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await JSRuntime.InvokeVoidAsync("breadCrumbStyle");
            }
            LoadBreadCrumbsPath();
            await base.OnAfterRenderAsync(firstRender);
        }

        private void LoadBreadCrumbsPath()
        {
            if (Artifact == null || FileService is null)
                return;

            if (_breadCrumbsPath.Length == 0)
            {
                _breadCrumbsPath = FileService.GetShowablePath(Artifact.FullPath).Trim().Split("/", StringSplitOptions.RemoveEmptyEntries);
                StateHasChanged();
                return;
            }
            _breadCrumbsPath = FileService.GetShowablePath(Artifact.FullPath).Trim().Split("/", StringSplitOptions.RemoveEmptyEntries);
        }

    }
}
