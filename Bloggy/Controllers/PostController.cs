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
  public class PostController(CosmosService cosmos, ImageService imageService, CompletionService completionService) : ControllerBase
  {
    [HttpGet()]
    public async Task<IActionResult> GetAll()
    {
      var posts = await cosmos.GetAllAsync<Post>(post => post.isTrending == false);
      posts = posts.OrderByDescending(post => post.created);
      var dto = new List<PostGetDto>();
      foreach (var post in posts)
      {
        // Get comments & authors that go with the post
        var comments = await cosmos.GetAllAsync<Comment>(comment => comment.postId == post.id);
        var authorIds = new HashSet<string>([post.ownerId]);
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

    [HttpGet("trending")]
    public async Task<IActionResult> GetTrending()
    {
      var posts = await cosmos.GetAllAsync<Post>(post => post.isTrending == true);
      posts = posts.OrderByDescending(post => post.created);
      var dto = new List<PostGetDto>();
      foreach (var post in posts)
      {
        // Get comments that go with the post
        var comments = await cosmos.GetAllAsync<Comment>(comment => comment.postId == post.id);
        var authorIds = new HashSet<string>([post.ownerId]);
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

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(string id)
    {
      var post = await cosmos.GetByIdAsync<Post>(id);
      if (post is not null)
      {
        // Get comments that go with post
        var comments = await cosmos.GetAllAsync<Comment>(comment => comment.postId == id);
        var authorIds = new HashSet<string>([post.ownerId]);
        foreach (var comment in comments)
        {
          authorIds.Add(comment.ownerId);
        }
        var authors = await cosmos.GetAllAsync<User>(user => authorIds.Contains(user.id));
        return Ok(new PostGetDto()
        {
          post = post,
          comments = comments,
          authors = authors,
        });
      }
      else
      {
        return NotFound();
      }
    }

    [Authorize]
    [HttpPost()]
    public async Task<IActionResult> Create([FromBody] PostCreateDto postDto)
    {
      var post = new Post(Guid.NewGuid().ToString(), User)
      {
        title = postDto.title,
        text = postDto.text,
        images = [],
        created = DateTime.Now,
        isTrending = false,
      };

      await cosmos.UpdateAsync(post, User);

      var author = await cosmos.GetByIdAsync<User>(AuthService.GetUserId(User));

      return Ok(new PostGetDto()
      {
        post = post,
        comments = [],
        authors = [author!],
      });
    }

    [Authorize]
    [HttpPost("{id}/add-image")]
    public async Task<IActionResult> AddImage(string id)
    {
      await cosmos.AuthorizeAsync<Post>(id, User);

      try
      {
        var imageUrl = await imageService.UploadImageAsync(new()
        {
          Raw = Request.Body,
          Name = $"post-{Guid.NewGuid()}",
          Width = 1080,
          Height = 1080,
        });

        var post = await cosmos.GetByIdAsync<Post>(id);
        if (post is not null)
        {
          post.images ??= [];
          post.images.Add(imageUrl);
          await cosmos.UpdateAsync(post, User);
        }
        else
        {
          return BadRequest("User does not exist");
        }

        return Ok(post);
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

    [Authorize]
    [HttpPost("{id}/add-comment")]
    public async Task<IActionResult> AddComment(string id, [FromBody] CommentCreateDto commentDto)
    {
      // Verify that the post exists
      var post = await cosmos.GetByIdAsync<Post>(id);
      if (post is null)
      {
        return BadRequest("Post does not exist");
      }

      if (post.isTrending is true)
      {
        // Create a completion instead of a real comment
        var @event = (await cosmos.GetAllAsync<Event>(e => e.trending != null && e.trending.id == post.id)).FirstOrDefault();
        var completion = await completionService.Create(new()
        {
          key = commentDto.text?.Trim() ?? "",
          eventId = @event.id,
          challengeId = null,
        }, User);

        return Ok(completion);
      }

      var comment = new Comment(Guid.NewGuid().ToString(), User)
      {
        postId = id,
        text = commentDto.text,
        created = DateTime.Now,
      };

      await cosmos.UpdateAsync(comment, User);

      return Ok(comment);
    }
  }
}