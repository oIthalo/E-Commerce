using ECommerceAPI.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
namespace ECommerceAPI.Service;

public static class TokenService
{
    public static string GenerateToken(User user)
    {
        var secretKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY");
        if (string.IsNullOrEmpty(secretKey))
        {
            throw new InvalidOperationException("A chave secreta JWT não está definida.");
        }
        var key = Encoding.ASCII.GetBytes(secretKey);

        var handler = new JwtSecurityTokenHandler();
        var credentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role.Name!.ToLower())
            }),
            SigningCredentials = credentials,
            Expires = DateTime.UtcNow.AddHours(2),
        };
        var token = handler.CreateToken(tokenDescriptor);
        return handler.WriteToken(token);
    }
}