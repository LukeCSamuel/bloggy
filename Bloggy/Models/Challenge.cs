namespace Bloggy.Models
{
  public class Challenge : CosmosModel, IAwardsPoints
  {
    public string? name { get; set; }
    public string? key { get; set; }
    public int? maxPoints { get; set; }

    public Challenge() : base("") { }
    public Challenge(string id) : base(id) { }
  }
}