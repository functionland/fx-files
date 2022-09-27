namespace Functionland.FxFiles.Client.Shared.Components
{
    public partial class EmptyListView
    {
        [Parameter]
        public string? Title { get; set; }

        [Parameter]
        public bool IsAddFileButtonVisible { get; set; }

        [Parameter]
        public bool IsAddFolderButtonVisible { get; set; } = true;

        [Parameter]
        public bool IsUploadButtonVisible { get; set; } = true;

        [Parameter]
        public EventCallback OnAddFileButtonClick { get; set; }

        [Parameter]
        public EventCallback OnAddFolderButtonClick { get; set; }

        [Parameter]
        public EventCallback OnUploadButtonClick { get; set; }
    }
}
