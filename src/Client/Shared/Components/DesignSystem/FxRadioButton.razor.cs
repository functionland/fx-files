namespace Functionland.FxFiles.Client.Shared.Components
{
    public partial class FxRadioButton
    {
        [Parameter]
        public string Title { get; set; }

        [Parameter]
        public bool? Checked { get; set; }

        [Parameter]
        public string Id { get; set; }

        [Parameter]
        public RadioBtnDisabledType RadioBtnDisabledType { get; set; }

        [Parameter]
        public string InputName { get; set; }

    }

    public enum RadioBtnDisabledType 
    {
        Checked,
        UnChecked
    }
}
