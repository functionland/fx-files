using Functionland.FxFiles.Client.Shared.Components.Modal;

namespace Functionland.FxFiles.Client.Shared.Components
{
    public partial class FxProgressBar
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
        public int Height { get; set; } = 4;

        [Parameter]
        public ProgressMode ProgressMode { get; set; }

        private double GetPercentComplete()
        {
            double result = 0;
            if (ProgressMax == 0)
                return result;

            if (ProgressCurrentValue == ProgressMax)
            {
                return 100;
            }
            result = (ProgressCurrentValue * 100 / ProgressMax);

            return result;
        }
    }
}
