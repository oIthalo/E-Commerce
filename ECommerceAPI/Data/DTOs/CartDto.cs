namespace ECommerceAPI.Data.DTOs;

public class CartDto
{
    public CartHeaderDto CartHeader { get; set; }
    public IEnumerable<CartItemDto> CartItems { get; set; }
}