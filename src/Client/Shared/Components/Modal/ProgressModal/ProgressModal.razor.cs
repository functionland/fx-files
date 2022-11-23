namespace Functionland.FxFiles.Client.Shared.Components.Modal;

public partial class ProgressModal
{
    [Parameter]
    public string CurrentText { get; set; } = default!;

    [Parameter]
    public string CurrentSubText { get; set; } = default!;

    [Parameter]
    public double ProgressCurrentValue { get; set; } = default;

    [Parameter]
    public int ProgressMax { get; set; }

    [Parameter]
    public EventCallback OnCancel { get; set; }

    private string Title { get; set; } = default!;
    private ProgressMode ProgressMode { get; set; }
    private bool IsCancellable { get; set; } = true;
    private bool _isModalOpen = false;

    public async Task ShowAsync(ProgressMode progressMode, string title, bool isCancelable)
    {
        ProgressMode = progressMode;
        Title = title;
        IsCancellable = isCancelable;
        _isModalOpen = true;
        await InvokeAsync(StateHasChanged);
    }

    public async Task RefreshAsync()
    {
        await InvokeAsync(StateHasChanged);
    }

    public async Task CloseAsync()
    {
        await OnCancel.InvokeAsync();
        _isModalOpen = false;
        await InvokeAsync(StateHasChanged);
    }

    private double GetPercentComplete()
    {
        double result = 0;
        if (ProgressMax == 0)
            return result;

        if (ProgressCurrentValue == ProgressMax)
        {
            return 100;
        }
        result = (ProgressCurrentValue * 100 / ProgressMax);

        return Math.Round(result);
    }
}
