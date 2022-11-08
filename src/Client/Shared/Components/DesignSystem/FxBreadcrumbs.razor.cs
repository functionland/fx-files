namespace Functionland.FxFiles.Client.Shared.Components
{
    public partial class FxBreadcrumbs
    {
        [Parameter] public FsArtifact? Artifact { get; set; }
        [Parameter] public IFileService? FileService { get; set; }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await JSRuntime.InvokeVoidAsync("breadCrumbStyle");
            }
            await base.OnAfterRenderAsync(firstRender);
        }

        private string[] GetBreadCrumbsPath(FsArtifact artifact)
        {
            if (FileService is null)
                throw new InvalidOperationException("");//TODo: 

            string[] _breadCrumbsPath = FileService.GetShowablePath(artifact.FullPath).Trim().Split("/", StringSplitOptions.RemoveEmptyEntries);

            return _breadCrumbsPath;
        }
    }
}
