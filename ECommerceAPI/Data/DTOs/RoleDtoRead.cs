using System.ComponentModel.DataAnnotations;
namespace ECommerceAPI.Data.DTOs;

public class RoleDtoRead
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
}