using System.ComponentModel.DataAnnotations;
namespace ECommerceAPI.Data.DTOs;

public class UserDtoUpdate
{
    public string Username { get; set; }
    [DataType(DataType.Password)]
    public string Password { get; set; }
}