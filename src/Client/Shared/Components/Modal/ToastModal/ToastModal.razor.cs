using System.Threading;
using System.Timers;

using Timer = System.Timers.Timer;

namespace Functionland.FxFiles.Client.Shared.Components.Modal
{
    public partial class ToastModal
    {
        private bool _isModalOpen = false;

        private string? _title { get; set; } = string.Empty;

        private string? _message { get; set; } = string.Empty;

        private FxToastType _toastType { get; set; }

        private Timer _timer = new Timer(5000);

        public void Show(string title, string message, FxToastType toastType)
        {
            _title = title;
            _message = message;
            _toastType = toastType;
            _isModalOpen = true;
            StateHasChanged();
            // Add timer for closing the modal after 5 seconds
            _timer.Elapsed += OnTimedEvent!;
            _timer.Enabled = true;
            StateHasChanged();
        }

        private void OnTimedEvent(Object source, System.Timers.ElapsedEventArgs e)
        {
            TimerHasChangedForCloseAsync();
        }
        private async Task TimerHasChangedForCloseAsync()
        {
            if (_isModalOpen)
            {
                await InvokeAsync(() =>
                {
                    Close();
                    StateHasChanged();
                    _timer.Stop();
                    _timer.Enabled = false;
                });
            }
        }
        private void Close()
        {
            _isModalOpen = false;
        }
    }
}
