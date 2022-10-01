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

        public void Show(string title, string message, FxToastType toastType)
        {
            _title = title;
            _message = message;
            _toastType = toastType;
            _isModalOpen = true;
            StateHasChanged();
            // Add timer for closing the modal after 5 seconds
            var timer = new Timer(5000);
            timer.Elapsed += OnTimedEvent!;
            timer.Enabled = true;
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
                });
            }
        }
        private void Close()
        {
            _isModalOpen = false;
        }
    }
}
