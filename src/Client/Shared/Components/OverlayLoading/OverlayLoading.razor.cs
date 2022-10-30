
namespace Functionland.FxFiles.Client.Shared.Components;
public partial class OverlayLoading
{
    [Parameter]
    public string ZIndex { get; set; } = "0";

    [Parameter]
    public LoadingType LoadingType { get; set; }

}

public enum LoadingType
{
    Overlay,
    Instead
}
