using ECommerceAPI.Models;
using System.ComponentModel.DataAnnotations;
namespace ECommerceAPI.Data.DTOs;

public class UserDtoLogin
{
    [Required]
    public string Username { get; set; }
    [Required]
    public string Password { get; set; }
}