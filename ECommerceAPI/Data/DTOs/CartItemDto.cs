namespace ECommerceAPI.Data.DTOs;

public class CartItemDto
{
    public Guid Id { get; set; }
    public ProductDtoCreate Product { get; set; }
    public int Quantity { get; set; }
    public Guid ProductId { get; set; }
    public Guid CartHeaderId { get; set; }
    public CartHeaderDto CartHeader { get; set; }
}