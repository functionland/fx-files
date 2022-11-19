namespace Functionland.FxFiles.Client.Shared.Components.Modal;

public partial class ProgressModal
{
    private string currentText = default!;
    [Parameter]
    public string CurrentText
    {
        get => currentText;
        set
        {
            if (currentText != value)
            {
                currentText = value;
            }
        }
    }

    private string currentSubText = default!;
    [Parameter]
    public string CurrentSubText
    {
        get => currentSubText;
        set
        {
            if (currentSubText != value)
            {
                currentSubText = value;
            }
        }
    }

    private double progressCurrentValue = default;
    [Parameter]
    public double ProgressCurrentValue
    {
        get => progressCurrentValue;
        set
        {
            if (progressCurrentValue != value)
            {
                progressCurrentValue = value;
            }
        }
    }

    private int progressMax;
    [Parameter] public int ProgressMax 
    { 
        get => progressMax;
        set
        {
            if (progressMax != value)
            {
                progressMax = value;
            }
        }
    }

    [Parameter] public EventCallback OnCancel { get; set; }

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
