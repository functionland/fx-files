using Microsoft.AspNetCore.Components.Web;

namespace Functionland.FxFiles.App.Components.FxToolBar
{
    public partial class FxToolBar
    {
        [Parameter] public string? Title { get; set; }
        [Parameter] public bool IsPinned { get; set; }
        [Parameter] public string? SubTitle { get; set; }
        [Parameter, EditorRequired] public bool IsInRoot { get; set; }
        [Parameter] public bool IsAddButtonVisible { get; set; }
        [Parameter] public bool IsBackButtonVisible { get; set; }
        [Parameter] public bool IsOverflowButtonVisible { get; set; }
        [Parameter] public EventCallback<MouseEventArgs> OnAddButtonClick { get; set; }
        [Parameter] public EventCallback<MouseEventArgs> OnOverflowButtonClick { get; set; }
        [Parameter] public EventCallback OnSearchFocused { get; set; }
        [Parameter] public EventCallback<string?> OnSearch { get; set; }

        [Parameter] public bool IsInSearchMode { get; set; }
        [Parameter] public EventCallback OnBackClick { get; set; }


        private async Task GoBack()
        {
            await JSRuntime.InvokeVoidAsync("history.back");
        }

        private void HandleSearchFocused()
        {
            OnSearchFocused.InvokeAsync();
        }

        private void HandleSearch(string? text)
        {
            OnSearch.InvokeAsync(text);
        }

        private void HandleBackClick()
        {
            OnBackClick.InvokeAsync();
        }
    }
}