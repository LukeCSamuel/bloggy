using Bloggy.Models;
using Bloggy.Models.Dto;
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
    [HttpGet("scores")]
    public async Task<IActionResult> GetAllUserCompletions()
    {
      var users = await cosmos.GetAllAsync<User>(u => u.isNpc == false);
      var completions = await cosmos.GetAllAsync<Completion>();
      var events = await cosmos.GetAllAsync<Event>();
      var challenges = await cosmos.GetAllAsync<Challenge>();
      var dtos = new List<UserCompletionDto>();

      // For each user, get their completions and add to DTOs
      foreach (var user in users)
      {
        var userCompletions = completions.Where(c => c.ownerId == user.id);
        var completionDtos = new List<SuccessfulCompletionDto>();
        foreach (var completion in userCompletions)
        {
          var @event = completion.eventId is not null ? events.FirstOrDefault(e => e.id == completion.eventId) : null;
          var challenge = completion.challengeId is not null ? challenges.FirstOrDefault(c => c.id == completion.challengeId) : null;
          completionDtos.Add(new()
          {
            name = @event?.name ?? challenge?.name,
            time = completion.time,
            pointsAwarded = completion.pointsAwarded,
          });
        }

        dtos.Add(new()
        {
          user = user,
          completions = completionDtos,
        });
      }

      return Ok(dtos);
    }

    [HttpGet("{id}")]
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
    [HttpPost("{id}")]
    public async Task<IActionResult> updateUser(string id, [FromBody] UserUpdateDto dto)
    {
      var user = await cosmos.GetByIdAsync<User>(id);
      if (user is null)
      {
        return NotFound("User does not exist");
      }

      user.about = dto.about ?? user.about;
      user.name = dto.name ?? user.name;
      user.pfpUrl = dto.pfpUrl ?? user.pfpUrl;
      await cosmos.UpdateAsync(user, User);

      return Ok(user);
    }


    [Authorize]
    [HttpPost("{id}/pfp")]
    public async Task<IActionResult> SetPfp(string id)
    {
      await cosmos.AuthorizeAsync<User>(id, User);

      try
      {
        var pfpUrl = await imageService.UploadImageAsync(new()
        {
          Raw = Request.Body,
          Name = $"pfp-{Guid.NewGuid()}",
          Width = 300,
          Height = 300
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

    [HttpGet("{id}/posts")]
    public async Task<IActionResult> getPosts(string id)
    {
      var authorIds = new HashSet<string>([id]);
      var posts = await cosmos.GetAllAsync<Post>(post => post.ownerId == id);
      posts = posts.OrderByDescending(post => post.created);
      var dto = new List<PostGetDto>();
      foreach (var post in posts)
      {
        // Get comments that go with the post
        var comments = await cosmos.GetAllAsync<Comment>(comment => comment.postId == post.id);
        foreach (var comment in comments)
        {
          authorIds.Add(comment.ownerId);
        }
        var authors = await cosmos.GetAllAsync<User>(user => authorIds.Contains(user.id));
        dto.Add(new()
        {
          post = post,
          comments = comments,
          authors = authors,
        });
      }

      return Ok(dto);
    }

    [HttpGet("{id}/completions")]
    public async Task<IActionResult> getCompletions(string id)
    {
      var completions = await cosmos.GetAllAsync<Completion>(completion => completion.ownerId == id);
      var dtos = new List<SuccessfulCompletionDto>();
      foreach (var completion in completions)
      {
        var @event = completion.eventId is not null ? await cosmos.GetByIdAsync<Event>(completion.eventId) : null;
        var challenge = completion.challengeId is not null ? await cosmos.GetByIdAsync<Challenge>(completion.challengeId) : null;
        dtos.Add(new()
        {
          name = @event?.name ?? challenge?.name,
          time = completion.time,
          pointsAwarded = completion.pointsAwarded,
        });
      }

      return Ok(dtos);
    }
  }
}