namespace Bloggy.Models
{
  internal class Completion : AuthorizedCosmosModel
  {
    public string? eventId { get; set; }
    public string? challengeId { get; set; }
    public DateTime? time { get; set; }
    public int? pointsAwarded { get; set; }

    public Completion() : base("", "") { }
    public Completion(string id, string ownerId) : base(id, ownerId) { }
  }
}