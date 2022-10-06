using Microsoft.AspNetCore.Components.Web;

using static System.Net.Mime.MediaTypeNames;

namespace Functionland.FxFiles.Client.Shared.Components
{
    public partial class FxToolBar
    {
        [Parameter] public string? Title { get; set; }
        [Parameter] public bool IsPinned { get; set; }
        [Parameter] public string? SubTitle { get; set; }
        [Parameter, EditorRequired] public bool IsInRoot { get; set; }
        [Parameter] public bool IsInSelectMode { get; set; } = false;
        [Parameter] public bool IsAddButtonVisible { get; set; } = true;
        [Parameter] public bool IsBackButtonVisible { get; set; } = true;
        [Parameter] public bool IsOverflowButtonVisible { get; set; } = true;
        [Parameter] public bool IsInFileBrowser { get; set; } = false;
        [Parameter] public EventCallback<MouseEventArgs> OnAddButtonClick { get; set; }
        [Parameter] public EventCallback<MouseEventArgs> OnOverflowButtonClick { get; set; }
        [Parameter] public EventCallback OnSearchFocused { get; set; }
        [Parameter] public EventCallback<string?> OnSearch { get; set; }

        [Parameter] public bool IsInSearchMode { get; set; }
        [Parameter] public EventCallback<MouseEventArgs> OnBackClick { get; set; }

        private FxSearchInput _searchInputRef = default!;

        private void HandleSearchFocused()
        {
            OnSearchFocused.InvokeAsync();
        }

        private async Task HandleSearch(string? text)
        {
            await InvokeAsync(async () =>
            {
                await OnSearch.InvokeAsync(text);
            });
        }

        private async Task HandleBackClick()
        {
            if (!IsInFileBrowser)
            {
                await JSRuntime.InvokeVoidAsync("history.back");
                return;
            }
            if (_searchInputRef != null)
            {
                _searchInputRef.HandleClearInputText();
            }
            await OnBackClick.InvokeAsync();
        }
    }
}