using System.ComponentModel.DataAnnotations;
namespace ECommerceAPI.Data.DTOs;

public class ProductDtoUpdate
{
    public string Name { get; set; }
    public string Description { get; set; }
    public double Price { get; set; }
    public Guid CategoryId { get; set; }
    public int Stock { get; set; }
    public string ImageUrl { get; set; }
}