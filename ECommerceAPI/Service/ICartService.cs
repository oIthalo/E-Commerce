using ECommerceAPI.Data.DTOs;
using ECommerceAPI.Models;
using Microsoft.AspNetCore.JsonPatch;
namespace ECommerceAPI.Service;

public interface ICartService
{
    Task<CartHeader> GetCartByUserId(Guid userId);
    Task AddCartItem(CartItemDtoCreate cartItemDto);
    Task RemoveCartItem(Guid itemId);
    Task ClearByCartId(Guid cartHeaderId);
    Task UpdateCartItem(Guid itemId, CartItemDtoUpdate cartItemDto);
}