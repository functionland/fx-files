﻿namespace Functionland.FxFiles.Client.Shared.Components.Modal;

public partial class ProgressModal
{
    [Parameter]
    public string CurrentText { get; set; } = default!;

    [Parameter]
    public string CurrentSubText { get; set; } = default!;

    [Parameter]
    public int ProgressCurrentValue { get; set; } = default;

    [Parameter]
    public int ProgressMax { get; set; } = 100;

    [Parameter]
    public EventCallback OnCancel { get; set; }

    private string _title { get; set; } = default!;
    private ProgressMode _progressMode { get; set; }
    private bool _isCancellable { get; set; } = true;
    private bool _isModalOpen = false;

    public async Task ShowAsync(ProgressMode progressMode, string title, bool isCanellabel)
    {
        _progressMode = progressMode;
        _title = title;
        _isCancellable = isCanellabel;
        _isModalOpen = true;
        await InvokeAsync(() => StateHasChanged());
    }

    public async Task CloseAsync()
    {
        await OnCancel.InvokeAsync();
        _isModalOpen = false;
        await InvokeAsync(() => StateHasChanged());
    }

    private double GetPrecentComplete()
    {
        double result = 0;
        if (ProgressMax != 0)
        {
            if (ProgressCurrentValue == ProgressMax)
            {
                return 100;
            }
            result = (ProgressCurrentValue * 100 / ProgressMax);
        }

        return result;
    }
}
