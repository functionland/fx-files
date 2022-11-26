namespace Functionland.FxFiles.Client.Shared.Pages;

public partial class SettingAbout
{
    protected override Task OnInitAsync()
    {
        GoBackService.SetState((Task () =>
        {
            HandleToolbarBack();
            StateHasChanged();
            return Task.CompletedTask;
        }), true, false);

        return base.OnInitAsync();
    }

    private void HandleToolbarBack()
    {
        NavigationManager.NavigateTo("settings", false, true);
    }
}
