namespace ECommerceAPI.Models;

public class CartItem
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public double SubTotal { get; set; }
    public int Quantity { get; set; }
    public Guid CartHeaderId { get; set; }
    public CartHeader CartHeader { get; set; }
}