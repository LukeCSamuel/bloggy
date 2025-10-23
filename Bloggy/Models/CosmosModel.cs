using System.Security.Claims;
using Bloggy.Services;

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

  public abstract class AuthorizedCosmosModel : CosmosModel
  {
    protected AuthorizedCosmosModel(string id, string ownerId) : base(id)
    {
      this.ownerId = ownerId;
    }

    protected AuthorizedCosmosModel(string id, ClaimsPrincipal user) : base(id)
    {
      this.ownerId = AuthService.GetUserId(user);
    }

    public virtual string ownerId { get; set; }
  }
}