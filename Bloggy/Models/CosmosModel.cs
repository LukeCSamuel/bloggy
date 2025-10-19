namespace Bloggy.Models
{
  internal abstract class CosmosModel
  {
    public required virtual string id { get; set; }
    public virtual string type
    {
      get => GetType().Name;
      set { }
    }
  }
}