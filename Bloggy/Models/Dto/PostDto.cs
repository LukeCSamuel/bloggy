namespace Bloggy.Models.Dto
{
  public class PostCreateDto
  {
    public string? title { get; set; }
    public string? text { get; set; }
  }

  public class CommentCreateDto
  {
    public string? text { get; set; }
  }

  internal class PostGetDto
  {
    public required Post post { get; set; }
    public required IEnumerable<Comment> comments { get; set; }
    public required IEnumerable<User> authors { get; set; }
  }
}
