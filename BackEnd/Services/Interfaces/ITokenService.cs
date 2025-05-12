using BackEnd.DTO;
using Microsoft.AspNetCore.Identity;

namespace BackEnd.Services.Interfaces
{
    public interface ITokenService
    {

        TokenDTO GenerateToken(IdentityUser user, List<string> roles);

    }
}
