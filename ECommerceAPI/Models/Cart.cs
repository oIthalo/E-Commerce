namespace ECommerceAPI.Models;

public class Cart
{
    public CartHeader CartHeader { get; set; }
    public IEnumerable<CartItem> CartItems { get; set; }
}