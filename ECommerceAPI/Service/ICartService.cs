using ECommerceAPI.Data.DTOs;
namespace ECommerceAPI.Service;

public interface ICartService
{
    Task<CartDto> GetCartByUserIdAsync(Guid userId);
    Task<CartDto> UpdateCartAsync(CartDto cartDto);
    Task<bool> CleanCartAsync(Guid userId);
    Task<bool> DeleteItemCartAsync(Guid cartItemId);
}