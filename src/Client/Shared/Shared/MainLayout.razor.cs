using Microsoft.AspNetCore.Components.Web;

namespace Functionland.FxFiles.Client.Shared.Shared;

public partial class MainLayout
{
    private bool _isLoading = true;

    protected override Task OnInitializedAsync()
    {
        _isLoading = false;
        return base.OnInitializedAsync();
    }
}
