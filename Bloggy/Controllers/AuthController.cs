using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Bloggy.Models;
using Bloggy.Models.Dto;
using Bloggy.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bloggy.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class AuthController(AuthService auth, CosmosService cosmos) : ControllerBase
  {
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto register)
    {
      var user = new User(Guid.NewGuid().ToString())
      {
        name = register.name,
        created = DateTime.Now,
        isNpc = false,
      };

      await cosmos.UpdateAsync(user, force: true);
      var token = auth.CreateUserToken(user);

      return Ok(new AuthDto
      {
        accessToken = auth.WriteToken(token),
        expiresAt = token.ValidTo
      });
    }

    [Authorize]
    [HttpGet("current-user")]
    public async Task<IActionResult> CurrentUser()
    {
      var id = AuthService.GetUserId(User);
      var user = await cosmos.GetByIdAsync<User>(id);

      return Ok(user);
    }
  }
}