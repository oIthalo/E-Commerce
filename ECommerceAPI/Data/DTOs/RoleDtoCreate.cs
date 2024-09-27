using System.ComponentModel.DataAnnotations;
namespace ECommerceAPI.Data.DTOs;

public class RoleDtoCreate
{
    public string Name { get; set; }
    public string Description { get; set; }
}