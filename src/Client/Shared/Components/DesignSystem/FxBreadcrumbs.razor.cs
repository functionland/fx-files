namespace Functionland.FxFiles.Client.Shared.Components
{
    public partial class FxBreadcrumbs
    {
        [Parameter] public FsArtifact? Artifact { get; set; }
        [Parameter] public IFileService? FileService { get; set; }

        private string[] GetBreadCrumbsPath(FsArtifact artifact)
        {
            if (FileService is null)
                throw new InvalidOperationException("");//TODo: 

            string[] _breadCrumbsPath = FileService.GetShowablePath(artifact.FullPath).Trim().Split("/", StringSplitOptions.RemoveEmptyEntries);

            return _breadCrumbsPath;
        }
    }
}
