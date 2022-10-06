namespace Functionland.FxFiles.Client.Shared.Pages;

public partial class HomePage
{
    protected override Task OnInitAsync()
    {
        if (IsiOS)
            NavigationManager.NavigateTo("settings", false, true);
        else
            NavigationManager.NavigateTo("mydevice", false, true);

        return base.OnInitAsync();
    }
}