using Microsoft.AspNetCore.Components.Web;

namespace Functionland.FxFiles.Client.Shared.Components
{
    public partial class FxCheckBox
    {
        [Parameter]
        public bool IsChecked { get; set; }

        [Parameter]
        public EventCallback<bool> IsCheckedChanged { get; set; }

        [Parameter]
        public bool IsEnable { get; set; } = true;

        [Parameter]
        public string Class { get; set; } = string.Empty;

        [Parameter]
        public EventCallback<MouseEventArgs> OnClick { get; set; }

        private void OnChange()
        {
            IsChecked = !IsChecked;
            IsCheckedChanged.InvokeAsync(IsChecked);
        }
    }
}
