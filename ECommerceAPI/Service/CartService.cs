using AutoMapper;
using ECommerceAPI.Data;
using ECommerceAPI.Data.DTOs;
using ECommerceAPI.Models;
using Microsoft.EntityFrameworkCore;
namespace ECommerceAPI.Service;

public class CartService : ICartService
{
    private ECommerceContext _context;
    private IMapper _mapper;

    public CartService(ECommerceContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<CartDto> GetCartByUserIdAsync(Guid userId)
    {
        Cart cart = new Cart
        { 
            CartHeader = await _context.CartHeaders.FirstOrDefaultAsync(x => x.UserId.Equals(userId)) 
        };
        cart.CartItems = _context.CartItems
            .Where(x => x.CartHeaderId.Equals(cart.CartHeader.Id))
            .Include(x => x.Product);

        return _mapper.Map<CartDto>(cart);
    }

    public async Task<bool> DeleteItemCartAsync(Guid cartItemId)
    {
        try
        {
            var cartItem = await _context.CartItems
                        .FirstOrDefaultAsync(x => x.Id.Equals(cartItemId));

            if (cartItem is null)
                throw new Exception("Cart item not found");

            int total = _context.CartItems
                .Where(x => x.CartHeaderId.Equals(cartItem.CartHeaderId))
                .Count();

            _context.CartItems.Remove(cartItem);

            if (total == 1)
            {
                var cartHeaderRemove = await _context.CartHeaders
                    .FirstOrDefaultAsync(x => x.Id.Equals(cartItem.CartHeaderId));

                _context.CartHeaders.Remove(cartHeaderRemove);
            }
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<bool> CleanCartAsync(Guid userId)
    {
        var cartHeader = await _context.CartHeaders
            .FirstOrDefaultAsync(x => x.UserId.Equals(userId));

        if (cartHeader is not null)
        {
            _context.CartItems.RemoveRange(_context.CartItems
                .Where(x => x.CartHeaderId.Equals(cartHeader.Id)));

            _context.CartHeaders.Remove(cartHeader);
            await _context.SaveChangesAsync();
            return true;
        }
        return false;
    }

    public async Task<CartDto> UpdateCartAsync(CartDto cartDto)
    {
        Cart cart = _mapper.Map<Cart>(cartDto);
        await SaveProductInDatabase(cartDto, cart);

        var cartHeader = await _context.CartHeaders.AsNoTracking().FirstOrDefaultAsync(
            x => x.UserId.Equals(cart.CartHeader.UserId));

        if (cartHeader is null)
            await CreateHeaderAndItem(cart);
        else
            await UpdateQuantityAndItems(cartDto, cart, cartHeader);

        return _mapper.Map<CartDto>(cart);

    }

    private async Task UpdateQuantityAndItems(CartDto cartDto, Cart cart, CartHeader? cartHeader)
    {
        var cartItem = await _context.CartItems.AsNoTracking().FirstOrDefaultAsync(
                               p => p.ProductId == cartDto.CartItems.FirstOrDefault()
                               .ProductId && p.CartHeaderId == cartHeader.Id);

        if (cartItem is null)
        {
            cart.CartItems.FirstOrDefault().CartHeaderId = cartHeader.Id;
            cart.CartItems.FirstOrDefault().Product = null;
            _context.CartItems.Add(cart.CartItems.FirstOrDefault());
            await _context.SaveChangesAsync();
        }
        else
        {
            cart.CartItems.FirstOrDefault().Product = null;
            cart.CartItems.FirstOrDefault().Quantity += cartItem.Quantity;
            cart.CartItems.FirstOrDefault().Id = cartItem.Id;
            cart.CartItems.FirstOrDefault().CartHeaderId = cartItem.CartHeaderId;
            _context.CartItems.Update(cart.CartItems.FirstOrDefault());
            await _context.SaveChangesAsync();
        }
    }

    public async Task SaveProductInDatabase(CartDto cartDto, Cart cart)
    {
        var product = await _context.Products.FirstOrDefaultAsync(
            x => x.Id.Equals(cartDto.CartItems.FirstOrDefault().ProductId));

        if (product is null)
        {
            _context.Products.Add(cart.CartItems.FirstOrDefault().Product);
            await _context.SaveChangesAsync();
        }
    }

    public async Task CreateHeaderAndItem(Cart cart)
    {
        _context.CartHeaders.Add(cart.CartHeader);
        await _context.SaveChangesAsync();

        cart.CartItems.FirstOrDefault().CartHeaderId = cart.CartHeader.Id;
        cart.CartItems.FirstOrDefault().Product = null;

        _context.CartItems.Add(cart.CartItems.FirstOrDefault());

        await _context.SaveChangesAsync();
    }
}