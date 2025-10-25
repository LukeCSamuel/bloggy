namespace Bloggy.Models
{
  public class Event : CosmosModel, IAwardsPoints
  {
    public string? name { get; set; }
    public string? key { get; set; }
    public int? maxPoints { get; set; }
    public DateTime? afterTime { get; set; }
    public string? afterEventId { get; set; }
    public Post? trending { get; set; }
    public List<EventEntity>? entities { get; set; }
    public bool? isFinale { get; set; }
    public bool? hasTriggered { get; set; }


    public Event() : base("") { }
    public Event(string id) : base(id) { }
  }

  public class EventEntity
  {
    public string? kind { get; set; }
    public string? data { get; set; }
  }
}