using System.Security.Claims;

namespace Bloggy.Models
{
  internal class Comment : AuthorizedCosmosModel
  {
    public string? postId { get; set; }
    public string? text { get; set; }
    public DateTime? created { get; set; }

    public Comment() : base("", "") { }

    public Comment(string id, ClaimsPrincipal user) : base(id, user) { }
  }
}