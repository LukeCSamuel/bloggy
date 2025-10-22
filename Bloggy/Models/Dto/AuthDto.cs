namespace Bloggy.Models.Dto
{
  internal class AuthDto
  {
    public string? accessToken { get; set; }
    public DateTime? expiresAt { get; set; }
  }
}