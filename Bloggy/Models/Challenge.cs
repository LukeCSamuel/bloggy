namespace Bloggy.Models
{
  internal class Challenge : CosmosModel
  {
    public string? name { get; set; }
    public string? key { get; set; }
    public int? maxPoints { get; set; }

    public Challenge() : base("") { }
    public Challenge(string id) : base(id) { }
  }
}