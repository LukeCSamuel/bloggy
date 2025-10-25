using System.Linq.Expressions;
using Bloggy.Models;
using Bloggy.Models.Dto;
using Bloggy.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bloggy.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class CompletionController(CompletionService completionService) : ControllerBase
  {
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CompletionRequestDto dto)
    {
      var completion = await completionService.Create(dto, User);
      return Ok(completion);
    }
  }
}