using Functionland.FxFiles.App.Components.DesignSystem;
using Functionland.FxFiles.App.Components.Modal;

namespace Functionland.FxFiles.App.Components.Modal
{
    public partial class ToastModal
    {
        private bool _isModalOpen;

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
        }

        private void Close()
        {
            _isModalOpen = false;
        }
    }
}
