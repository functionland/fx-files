namespace Functionland.FxFiles.Client.Shared.Components
{
    public partial class FxBreadcrumbs
    {
        [Parameter] public FsArtifact? Artifact { get; set; }

        private string[]? _breadCrumbsPath;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            LoadBreadCrumbsPath();
            await JSRuntime.InvokeVoidAsync("Amin");
            await base.OnAfterRenderAsync(firstRender);
        }
        private void LoadBreadCrumbsPath()
        {
            if (Artifact != null)
            {
                _breadCrumbsPath = Artifact.ShowablePath.Trim().Split("/", StringSplitOptions.RemoveEmptyEntries);
            }
        }

    }
}
