

namespace Bloggy.Models
{
  internal class User : AuthorizedCosmosModel
  {
    public string? name { get; set; }
    public string? pfpUrl { get; set; }
    public DateTime? created { get; set; }
    public bool? isNpc { get; set; }
    public string? about { get; set; }

    public User() : base("", "") { }

    public User(string id) : base(id, id) { }
  }
}
