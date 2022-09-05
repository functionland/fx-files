using Functionland.FxFiles.Api.Models.Account;
using Functionland.FxFiles.Shared.Dtos.Account;

namespace Functionland.FxFiles.Api.Services.Contracts;

public interface IJwtService
{
    Task<SignInResponseDto> GenerateToken(User user);
}
