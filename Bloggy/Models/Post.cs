using System.Security.Claims;

namespace Bloggy.Models
{
  public class Post : AuthorizedCosmosModel
  {
    public string? title { get; set; }
    public List<string>? images { get; set; }
    public string? text { get; set; }
    public DateTime? created { get; set; }
    public bool? isTrending { get; set; }

    public Post() : base("", "") { }

    public Post(string id, ClaimsPrincipal user) : base(id, user) { }
  }
}