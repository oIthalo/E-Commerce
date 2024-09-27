namespace ECommerceAPI.Data.DTOs;

public class ProductDtoRead
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public CategoryDtoRead Category { get; set; }
    public double Price { get; set; }
    public int Stock { get; set; }
    public string ImageUrl { get; set; }
    public DateTime PublicationDate { get; set; }
}