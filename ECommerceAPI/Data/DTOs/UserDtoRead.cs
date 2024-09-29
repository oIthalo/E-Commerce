using ECommerceAPI.Models;
namespace ECommerceAPI.Data.DTOs;

public class UserDtoRead
{
    public Guid Id { get; set; }
    public string Username { get; set; }
    public string Role { get; set; }
}