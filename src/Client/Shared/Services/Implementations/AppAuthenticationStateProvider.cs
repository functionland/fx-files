namespace Functionland.FxFiles.Client.Shared.Services.Implementations;

public partial class AppAuthenticationStateProvider : AuthenticationStateProvider
{
    public async Task RaiseAuthenticationStateHasChanged()
    {
        NotifyAuthenticationStateChanged(Task.FromResult(await GetAuthenticationStateAsync()));
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        return NotSignedIn();
    }

    public async Task<bool> IsUserAuthenticated()
    {
        return (await GetAuthenticationStateAsync()).User.Identity?.IsAuthenticated == true;
    }

    AuthenticationState NotSignedIn()
    {
        return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
    }
}
