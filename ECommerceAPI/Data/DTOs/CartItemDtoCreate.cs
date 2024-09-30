using ECommerceAPI.Models;
namespace ECommerceAPI.Data.DTOs;

public class CartItemDtoCreate
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
    public Guid CartHeaderId { get; set; }
}