using Microsoft.AppCenter.Channel;
using System.Threading;
using System.Timers;
using Timer = System.Timers.Timer;

namespace Functionland.FxFiles.Client.Shared.Components
{
    public partial class FxToast : IDisposable
    {

        public FxToastType ToastType { get; set; }

        public string? Title { get; set; }

        public string? Description { get; set; }

        public bool IsOpen { get; set; } = false;

        public EventCallback<bool> IsOpenChanged { get; set; }

        private static event Func<string, string, FxToastType, Task> OnShow = default!;
        private Timer _timer = new Timer(1000000);

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
            Title = title;
            Description = message;
            ToastType = toastType;
            IsOpen = true;
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
            if (IsOpen)
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
            IsOpen = false;
            IsOpenChanged.InvokeAsync(IsOpen);
        }

        public void Dispose()
        {
            OnShow -= HandleShow;
            _timer.Elapsed -= OnTimedEvent!;
        }

    }

    public enum FxToastType
    {
        Success,
        Warning,
        Info,
        Error
    }
}
