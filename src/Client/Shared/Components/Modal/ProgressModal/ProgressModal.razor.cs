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
        public string CurrentText { get; set; } = default!;

        [Parameter]
        public string CurrentSubText { get; set; } = default!;

        [Parameter]
        public int ProgressCurrentValue { get; set; }

        [Parameter]
        public int ProgressMax { get; set; }

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
            StateHasChanged();
        }

        public async Task CloseAsync()
        {
            await OnCancel.InvokeAsync();
            _isModalOpen = false;
        }
    }
}
