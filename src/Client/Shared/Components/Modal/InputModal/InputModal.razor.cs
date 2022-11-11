namespace Functionland.FxFiles.Client.Shared.Components.Modal;

public partial class InputModal
{
    private TaskCompletionSource<InputModalResult>? _tcs;
    private bool _isModalOpen;
    private string? _title;
    private string? _placeholder;
    private string? _inputValue;
    private string? _headTitle;
    private string? _doneBtnText;
    private FxTextInput? _inputRef;
    private string? _lable;

    public async Task<InputModalResult> ShowAsync(string tilte, string headTitle, string inputValue, string placeholder, string? doneBtnText = null, string? lable = null)
    {
        GoBackService.OnInit((Task () =>
        {
            Close();
            StateHasChanged();
            return Task.CompletedTask;
        }), true, false);

        _headTitle = headTitle;
        _inputValue = inputValue;
        _title = tilte;
        _placeholder = placeholder;
        _doneBtnText = doneBtnText ?? Localizer[nameof(AppStrings.Confirm)];
        _lable = lable ?? Localizer[nameof(AppStrings.Name)];

        _tcs?.SetCanceled();
        _isModalOpen = true;
        StateHasChanged();

        var timer = new System.Timers.Timer(700);
        timer.Elapsed += TimerElapsed;
        timer.Enabled = true;
        timer.Start();

        _tcs = new TaskCompletionSource<InputModalResult>();
        return await _tcs.Task;
    }

    private void TimerElapsed(object? sender, System.Timers.ElapsedEventArgs e)
    {
        InvokeAsync(async () =>
        {
            if (_inputRef is not null)
            {
                await _inputRef.FocusInputAsync();
            }
        });

        var timer = (System.Timers.Timer)sender;
        timer.Elapsed -= TimerElapsed;
        timer.Enabled = false;
        timer.Stop();

        timer.Dispose();
    }

    private void Close()
    {
        var result = new InputModalResult
        {
            ResultType = InputModalResultType.Cancel
        };

        try
        {
            _tcs?.SetResult(result);
        }
        finally
        {
            _tcs = null;

            _isModalOpen = false;
        }

    }

    private void Confirm()
    {
        var result = new InputModalResult
        {
            ResultType = InputModalResultType.Confirm,
            Result = _inputValue
        };

        try
        {
            _tcs?.SetResult(result);
        }
        finally
        {
            _tcs = null;

            _isModalOpen = false;
        }
    }

    public void Dispose()
    {
        _tcs?.SetCanceled();
    }
}