using Bloggy.Models;
using Bloggy.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SixLabors.ImageSharp;

namespace Bloggy.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class UserController(CosmosService cosmos, ImageService imageService) : ControllerBase
  {
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(string id)
    {
      var user = await cosmos.GetByIdAsync<User>(id);
      if (user is not null)
      {
        return Ok(user);
      }
      else
      {
        return NotFound();
      }
    }

    [Authorize]
    [HttpPost("{id:guid}/pfp")]
    public async Task<IActionResult> SetPfp (string id)
    {
      await cosmos.AuthorizeAsync<User>(id, User);

      try
      {
        var pfpUrl = await imageService.UploadImageAsync(new()
        {
          Raw = Request.Body,
          Name = $"pfp-{Guid.NewGuid()}",
          Width = 150,
          Height = 150
        });

        var user = await cosmos.GetByIdAsync<User>(id);
        if (user is not null)
        {
          user.pfpUrl = pfpUrl;
          await cosmos.UpdateAsync(user, User);
        }
        else
        {
          return BadRequest("User does not exist");
        }

        return Ok(user);
      }
      catch (UnknownImageFormatException)
      {
        return BadRequest("Unknown or unsupported image format");
      }
      catch (EmptyRequestBodyException)
      {
        return BadRequest("No image was provided");
      }

    }
  }
}