using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Functionland.FxFiles.Client.Shared.Components.Modal
{
    public partial class ProgressModal
    {
        [Parameter]
        public ProgressMode ProgressMode { get; set; }
        [Parameter]
        public string Title { get; set; } = default!;
        [Parameter]
        public string CurrentText { get; set; } = default!;
        [Parameter]
        public string CurrentSubText { get; set; } = default!;
        [Parameter]
        public int ProgressCurrentValue { get; set; }
        [Parameter]
        public int ProgressMax { get; set; }
        [Parameter]
        public bool IsCancellable { get; set; } = true;
        [Parameter]
        public EventCallback OnCancel { get; set; }

        private bool _isModalOpen = false;

        public async Task ShowAsync()
        {
            _isModalOpen = true;
            StateHasChanged();
        }

        public async Task CloseAsync()
        {
            await OnCancel.InvokeAsync();
            _isModalOpen = false;
        }
    }
}
