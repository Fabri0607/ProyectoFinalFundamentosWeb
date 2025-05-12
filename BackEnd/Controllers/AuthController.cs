using BackEnd.DTO;
using BackEnd.Services.Implementations;
using BackEnd.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    /*
     https://prod.liveshare.vsengsaas.visualstudio.com/join?7B52DF3F60603A16882F736D70FC63F1D607
     */
    public class AuthController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        ITokenService _tokenService;

        public AuthController(ITokenService tokenService,
                        UserManager<IdentityUser> userManager)
        {
            _tokenService = tokenService;
            _userManager = userManager;
        }

        [HttpPost]
        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO model)
        {


            IdentityUser user = await _userManager.FindByNameAsync(model.UserName);
            LoginDTO Usuario = new LoginDTO();
            if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
            {

                var userRoles = await _userManager.GetRolesAsync(user);

                var jwtToken = _tokenService.GenerateToken(user, userRoles.ToList());

                Usuario.Token = jwtToken;
                Usuario.Roles = userRoles.ToList();
                Usuario.UserName = user.UserName;


                return Ok(Usuario);
            }

            return Unauthorized();



        }



        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO model)
        {

            var userExists = await _userManager.FindByNameAsync(model.UserName);

            if (userExists != null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            IdentityUser user = new IdentityUser
            {
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.UserName
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);

            }

            return Ok();

        }
    }
}
