namespace Functionland.FxFiles.Client.Shared.Components
{
    public partial class FxAvatar
    {
        [Parameter, EditorRequired]
        public FxAvatarSizeEnum FxAvatarSizeType { get; set; }

        [Parameter]
        public FxAvatarStatusEnum FxAvatarStatusType { get; set; }
    }

    public enum FxAvatarSizeEnum
    {
        XSmall,
        Small,
        Medium,
        Large,
        XLarge
    }

    public enum FxAvatarStatusEnum
    {
        NoStatus,
        Selected,
        DeSelected,
        Edit
    }
}
