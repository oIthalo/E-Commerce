namespace ECommerceAPI.Data.DTOs;

public class ProductDtoCreate
{
    public string Name { get; set; }
    public string Description { get; set; }
    public Guid CategoryId { get; set; }
    public double Price { get; set; }
    public int Stock { get; set; }
    public string ImageUrl { get; set; }
}