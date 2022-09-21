using System.ComponentModel;
using System.Linq;

using Functionland.FxFiles.App.Components.Common;
using Functionland.FxFiles.App.Components.Modal;

using Microsoft.AspNetCore.Components.Web;

namespace Functionland.FxFiles.App.Components
{
    public partial class FxListView
    {
        [Parameter]
        public string ListTitle { get; set; } = String.Empty;

        [Parameter]
        public bool HasSearchInput { get; set; } = false;

        [Parameter, EditorRequired]
        public List<FsArtifact>? ItemsSource { get; set; }

        //[Parameter]
        public List<FsArtifact>? SelectedListItems { get; set; } = new List<FsArtifact>();
        //[Parameter]
        //public EventCallback<List<FsArtifact>?> SelectedListItemsChanged { get; set; }

        private ArtifactSelectionModal _artifactSelectionModalRef = default!;

        public ViewModeEnum ViewMode = ViewModeEnum.list;
        public SortOrderEnum SortOrder = SortOrderEnum.asc;

        public bool IsSelected;
        public bool IsSelectionMode;
        public bool IsSelectedAll = false;
        public DateTimeOffset PointerDownTime;

        public void ToggleSortOrder()
        {
            if (SortOrder == SortOrderEnum.asc)
            {
                SortOrder = SortOrderEnum.desc;
            }
            else
            {
                SortOrder = SortOrderEnum.asc;
            }

            //todo: change order of list items
        }

        public void OnSortChange()
        {
            //todo: Open sort bottom sheet
        }

        public void ToggleSelectedAll()
        {
            IsSelectedAll = !IsSelectedAll;
            //todo: select all items
        }

        public void ChangeViewMode(ViewModeEnum mode)
        {
            ViewMode = mode;
        }

        public void GoIntoItem()
        {
            //todo: go into folder or open file preview if it's file
        }

        public void OpenItemOverFlow()
        {
            //todo: open folder of file overflow bottom sheet
        }

        public void CancelSelection()
        {
            IsSelectionMode = false;
            SelectedListItems = new List<FsArtifact>();
        }

        public void PointerDown()
        {
            PointerDownTime = DateTimeOffset.UtcNow;
        }

        public void PointerUp()
        {
            if (!IsSelectionMode)
            {
                var downTime = (DateTimeOffset.UtcNow.Ticks - PointerDownTime.Ticks) / TimeSpan.TicksPerMillisecond;
                IsSelectionMode = downTime > 400;
            }
        }

        public void OnSelectionChanged(FsArtifact selectedItem)
        {
            if (SelectedListItems.Any(item => item.FullPath == selectedItem.FullPath))
            {
                SelectedListItems.Remove(selectedItem);
            }
            else
            {
                SelectedListItems.Add(selectedItem);
            }
        }
    }
}