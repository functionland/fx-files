using System.Threading;
using System.Timers;

using Timer = System.Timers.Timer;

namespace Functionland.FxFiles.Client.Shared.Components.Modal
{
    public partial class ToastModal : IDisposable
    {
        private static event Func<string, string, FxToastType, Task> OnShow = default!;
        private bool _isModalOpen = false;
        private string? _title { get; set; } = string.Empty;
        private string? _message { get; set; } = string.Empty;
        private FxToastType _toastType { get; set; }
        private Timer _timer = new Timer(5000);

        protected override Task OnInitAsync()
        {
            OnShow += HandleShow;
            return base.OnInitAsync();
        }

        public static void Show(string title, string message, FxToastType toastType)
        {
            if (OnShow is not null)
                OnShow.Invoke(title, message, toastType);
        }

        public async Task HandleShow(string title, string message, FxToastType toastType)
        {
            _title = title;
            _message = message;
            _toastType = toastType;
            _isModalOpen = true;
            _timer.Elapsed += OnTimedEvent!;
            _timer.Enabled = true;
            await InvokeAsync(() => StateHasChanged());
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

        public void Dispose()
        {
            OnShow -= HandleShow;
            _timer.Elapsed -= OnTimedEvent!;
        }
    }
}
