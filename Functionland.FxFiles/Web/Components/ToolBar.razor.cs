namespace Functionland.FxFiles.App.Components;

public partial class ToolBar
{
    [Parameter]
    public string? Title { get; set; }

    [Parameter]
    public string? SubTitle { get; set; }

    [Parameter, EditorRequired]
    public bool IsMainToolBar { get; set; }

    [Parameter]
    public bool IsAddButtonVisible { get; set; }

    [Parameter]
    public bool IsBackButtonVisible { get; set; }

    [Parameter]
    public bool IsOverflowButtonVisible { get; set; }

    public async Task GoBack()
    {
        await JSRuntime.InvokeVoidAsync("history.back");
    }
}
