namespace Functionland.FxFiles.Client.Shared.Pages;

public partial class HomePage
{
    protected override Task OnInitAsync()
    {
        if (IsiOS)
            NavigationManager.NavigateTo("settings");
        else
            NavigationManager.NavigateTo("mydevice");

        return base.OnInitAsync();
    }
}