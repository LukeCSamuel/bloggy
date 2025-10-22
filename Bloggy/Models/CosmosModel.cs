namespace Bloggy.Models
{
  public abstract class CosmosModel(string id)
  {
    public virtual string id { get; set; } = id;
    public virtual string type
    {
      get => GetType().Name;
      set { }
    }
  }

  public abstract class AuthorizedCosmosModel(string id, string ownerId) : CosmosModel(id)
  {
    public virtual string ownerId { get; set; } = ownerId;
  }
}