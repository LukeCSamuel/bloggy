using System.IdentityModel.Tokens.Jwt;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Bloggy.Models;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.IdentityModel.Tokens;

namespace Bloggy.Services
{
  public class AuthService
  {
    private readonly SigningCredentials _creds;
    private readonly string _issuer;
    private readonly JwtSecurityTokenHandler _tokenHandler = new();


    public AuthService(EnvService env)
    {
      var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(env.JwtKey));
      _creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
      _issuer = env.AppSettings.Jwt.Issuer;
    }

    /// <summary>
    /// Writes the given token into a JWT string
    /// </summary>
    public string WriteToken(JwtSecurityToken token)
    {
      return _tokenHandler.WriteToken(token);
    }

    /// <summary>
    /// Creates an auth token from the specified claims
    /// </summary>
    /// <param name="claims">List of claims to add to the token</param>
    public JwtSecurityToken GetToken(IEnumerable<Claim> claims)
    {
      return new JwtSecurityToken(
        issuer: _issuer,
        audience: _issuer,
        // Make the token last a year because I don't feel like implementing refresh
        expires: DateTime.Now.AddDays(365),
        claims: claims,
        signingCredentials: _creds
      );
    }

    internal JwtSecurityToken CreateUserToken (User user)
    {
      return GetToken([
        GetClaim(() => user.name),
        new Claim(JwtRegisteredClaimNames.Sub, user.id),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
      ]);
    }

    public static string GetUserId(ClaimsPrincipal user)
    {
      return user.Claims.Single(claim => claim.Type == ClaimTypes.NameIdentifier || claim.Type == JwtRegisteredClaimNames.Sub).Value;
    }

    public static string GetClaimType<T>(Expression<Func<T>> expression)
    {
      return expression.Body switch
      {
        MemberExpression { Member.DeclaringType: Type } ex => $"bloggy:{ex.Member.DeclaringType.FullName}.{ex.Member.Name}",
        _ => throw new Exception($"Cannot infer the claim type for expression \"{expression.Print()}\"")
      };
    }

    public static Claim GetClaim<T>(Expression<Func<T>> expression)
    {
      var type = GetClaimType(expression);
      var value = JsonSerializer.Serialize(expression.Compile().Invoke());
      return new Claim(type, value);
    }
  }
}