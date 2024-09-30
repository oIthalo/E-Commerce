using ECommerceAPI.Models;
namespace ECommerceAPI.Data.DTOs;

public class CartItemDto
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public string ProductName { get; set; }
    public int Quantity { get; set; }
    public Guid CartHeaderId { get; set; }
}