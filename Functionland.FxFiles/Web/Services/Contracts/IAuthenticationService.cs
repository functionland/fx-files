using Functionland.FxFiles.Shared.Dtos.Account;

namespace Functionland.FxFiles.App.Services.Contracts;

public interface IAuthenticationService
{
    Task SignIn(SignInRequestDto dto);

    Task SignOut();
}
