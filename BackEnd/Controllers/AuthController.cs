using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using BackEnd.DTO;
using BackEnd.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Entities.Entities;

namespace BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ITokenService _tokenService;

        public AuthController(
            ITokenService tokenService,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _tokenService = tokenService;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO model)
        {
            var user = await _userManager.FindByNameAsync(model.UserName);
            if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
            {
                var userRoles = await _userManager.GetRolesAsync(user);
                var jwtToken = _tokenService.GenerateToken(user, userRoles.ToList());

                return Ok(new LoginDTO
                {
                    UserName = user.UserName,
                    Token = jwtToken,
                    Roles = userRoles.ToList(),
                    MustChangePassword = user.MustChangePassword
                });
            }

            return Unauthorized(new { message = "Usuario o contraseña incorrectos" });
        }

        [HttpPost]
        [Route("register")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userExists = await _userManager.FindByNameAsync(model.UserName);
            if (userExists != null)
            {
                return BadRequest(new { message = "El usuario ya existe" });
            }

            var user = new ApplicationUser
            {
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.UserName,
                MustChangePassword = true // Set flag
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                return BadRequest(new { message = "Error al crear el usuario", errors = result.Errors });
            }

            if (!string.IsNullOrEmpty(model.Role) && await _roleManager.RoleExistsAsync(model.Role))
            {
                await _userManager.AddToRoleAsync(user, model.Role);
            }
            else
            {
                return BadRequest(new { message = "Rol inválido o no especificado" });
            }

            return Ok(new { message = "Usuario creado exitosamente" });
        }

        [HttpGet]
        [Route("users")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _userManager.Users.ToListAsync();
            var userDtos = new List<object>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userDtos.Add(new
                {
                    id = user.Id,
                    userName = user.UserName,
                    email = user.Email,
                    role = roles.FirstOrDefault()
                });
            }

            return Ok(userDtos);
        }

        [HttpPost]
        [Route("ChangePassword")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDTO model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (model.NewPassword != model.ConfirmPassword)
            {
                return BadRequest(new { message = "Las contraseñas nuevas no coinciden" });
            }

            var user = await _userManager.FindByNameAsync(model.UserName);
            if (user == null)
            {
                return NotFound(new { message = "Usuario no encontrado" });
            }

            if (!await _userManager.CheckPasswordAsync(user, model.TemporaryPassword))
            {
                return Unauthorized(new { message = "Contraseña temporal incorrecta" });
            }

            if (!user.MustChangePassword)
            {
                return BadRequest(new { message = "No se requiere cambio de contraseña" });
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, model.NewPassword);
            if (!result.Succeeded)
            {
                return BadRequest(new { message = "Error al cambiar la contraseña", errors = result.Errors });
            }

            user.MustChangePassword = false;
            await _userManager.UpdateAsync(user);

            return Ok(new { message = "Contraseña cambiada exitosamente" });
        }

        [HttpDelete]
        [Route("users/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound(new { message = "Usuario no encontrado" });
            }

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                return BadRequest(new { message = "Error al eliminar usuario", errors = result.Errors });
            }

            return Ok(new { message = "Usuario eliminado exitosamente" });
        }

        [HttpPut]
        [Route("Users/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] UpdateUserDTO model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound(new { message = "Usuario no encontrado" });
            }

            user.UserName = model.UserName;
            user.Email = model.Email;

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                return BadRequest(new { message = "Error al actualizar usuario", errors = updateResult.Errors });
            }

            var currentRoles = await _userManager.GetRolesAsync(user);
            if (currentRoles.Any() && currentRoles.First() != model.Role)
            {
                await _userManager.RemoveFromRolesAsync(user, currentRoles);
                if (!string.IsNullOrEmpty(model.Role) && await _roleManager.RoleExistsAsync(model.Role))
                {
                    await _userManager.AddToRoleAsync(user, model.Role);
                }
                else
                {
                    return BadRequest(new { message = "Rol inválido" });
                }
            }

            if (!string.IsNullOrEmpty(model.Password))
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var passwordResult = await _userManager.ResetPasswordAsync(user, token, model.Password);
                if (!passwordResult.Succeeded)
                {
                    return BadRequest(new { message = "Error al actualizar contraseña", errors = passwordResult.Errors });
                }
                user.MustChangePassword = true; // Require password change after admin reset
            }

            return Ok(new { message = "Usuario actualizado exitosamente" });
        }
    }
}