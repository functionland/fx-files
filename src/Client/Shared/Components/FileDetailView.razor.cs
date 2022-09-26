namespace Functionland.FxFiles.Client.Shared.Components
{
    public partial class FileDetailView
    {
        [Parameter]
        public bool IsImageFile { get; set; } = false;

        [Parameter]
        public string? FileName { get; set; }

        [Parameter]
        public string? FileDetailAreaClass { get; set; }

        [Parameter]
        public string? ContentBodyClass { get; set; }

        [Parameter]
        public FsArtifactType ArtifactType { get; set; }

        [Parameter]
        public RenderFragment? FileDetailItemFragment { get; set; } = default!;

        [Parameter]
        public RenderFragment? FileDetailItemActionsFragment { get; set; } = default!;

        [Parameter]
        public RenderFragment? FileDetailBottomActionFragment { get; set; } = default!;
    }
}
