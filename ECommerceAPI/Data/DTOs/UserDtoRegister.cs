using ECommerceAPI.Models;
namespace ECommerceAPI.Data.DTOs;

public class UserDtoRegister
{
    public string Username { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
}