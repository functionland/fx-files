namespace Functionland.FxFiles.App.Components;

public partial class FxToolBar
{
    [Parameter]
    public string? Title { get; set; }

    [Parameter, EditorRequired]
    public bool IsMainToolBar { get; set; }

    [Parameter]
    public bool IsBackButtonVisible { get; set; }

    public async Task GoBack()
    {
        await JSRuntime.InvokeVoidAsync("history.back");
    }
}
