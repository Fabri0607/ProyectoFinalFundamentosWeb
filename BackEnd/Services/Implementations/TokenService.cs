using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BackEnd.DTO;
using BackEnd.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace BackEnd.Services.Implementations
{
    public class TokenService : ITokenService
    {

        private IConfiguration _configuration;


        public TokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public TokenDTO GenerateToken(IdentityUser user, List<string> roles)
        {
            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())


            };

            foreach (var role in roles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, role));
            }


            var authSingKey = new SymmetricSecurityKey(Encoding
                                                        .UTF8
                                                        .GetBytes(_configuration["JWT:Key"]));


            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddDays(1),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSingKey, SecurityAlgorithms.HmacSha256)

                );
            var resultado = new JwtSecurityTokenHandler().WriteToken(token);


            return new TokenDTO
            {
                Token = resultado,
                Expiration = token.ValidTo
            };


        }
    }
}
