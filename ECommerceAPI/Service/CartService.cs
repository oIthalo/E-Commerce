using AutoMapper;
using ECommerceAPI.Data;
using ECommerceAPI.Data.DTOs;
using ECommerceAPI.Models;
using ECommerceAPI.Service;
using Microsoft.AspNetCore.JsonPatch;
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

    public async Task<CartHeader> GetCartByUserId(Guid userId)
    {
        var cartHeader = await _context.CartHeaders
            .FirstOrDefaultAsync(x => x.UserId.Equals(userId));

        if (cartHeader is null)
           await CreateCartHeader(userId);

        return cartHeader;
    }

    public async Task AddCartItem(CartItemDtoCreate cartItemDto)
    {
        CartItem cartItem = _mapper.Map<CartItem>(cartItemDto);

        if (cartItem.Quantity <= 0)
            throw new Exception("Quantity must be greater than zero");

        var product = await _context.Products
            .FirstOrDefaultAsync(x => x.Id.Equals(cartItem.ProductId));

        if (product is null)
            throw new Exception("Product not found");

        cartItem.SubTotal = product.Price * cartItem.Quantity;

        await _context.CartItems.AddAsync(cartItem);
        await _context.SaveChangesAsync();
    }

    public async Task ClearByCartId(Guid cartHeaderId)
    {
        var cartHeader = await _context.CartHeaders
            .FirstOrDefaultAsync(x => x.Id.Equals(cartHeaderId));

        var cartItems = await _context.CartItems
            .Where(x => x.CartHeaderId.Equals(cartHeaderId))
            .ToListAsync();

        _context.RemoveRange(cartItems);
        _context.CartHeaders.Remove(cartHeader);
        await _context.SaveChangesAsync();
    }

    public async Task RemoveCartItem(Guid itemId)
    {
        var cartItem = await _context.CartItems
            .FirstOrDefaultAsync(x => x.Id.Equals(itemId));

        if (cartItem is null)
            throw new Exception("Cart item not found");

        _context.CartItems.Remove(cartItem);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateCartItem(Guid itemId, CartItemDtoUpdate cartItemDto)
    {
        var cartItem = await _context.CartItems
            .FirstOrDefaultAsync(x => x.Id.Equals(itemId));

        if (cartItem is null)
            throw new Exception("Cart item not found");

        if (cartItemDto.Quantity > 0)
            cartItem.Quantity = cartItemDto.Quantity;
        else
            throw new Exception("Quantity must be greater than zero");

        var product = await _context.Products
            .FirstOrDefaultAsync(x => x.Id.Equals(cartItem.ProductId));

        if (product is null)
            throw new Exception("Product not found");

        cartItem.SubTotal = product.Price * cartItem.Quantity;

        _context.CartItems.Update(cartItem);
        await _context.SaveChangesAsync();
    }

    public async Task PatchCartItem(Guid itemId, JsonPatchDocument<CartItemDtoUpdate> patchDoc)
    {
        var cartItem = await _context.CartItems
            .FirstOrDefaultAsync(x => x.Id.Equals(itemId));

        var cartItemToUpdate = _mapper.Map<CartItemDtoUpdate>(cartItem);

        if (cartItem is null)
            throw new Exception("Cart item not found");

        patchDoc.ApplyTo(cartItemToUpdate);

        if (cartItem.Quantity <= 0)
            throw new Exception("Quantity must be greater than zero");

        var product = await _context.Products
            .FirstOrDefaultAsync(x => x.Id.Equals(cartItem.ProductId));

        if (product is null)
            throw new Exception("Product not found");

        cartItem.SubTotal = product.Price * cartItem.Quantity;

        _context.CartItems.Update(cartItem);
        await _context.SaveChangesAsync();
    }

    // lógica para criar um cart header caso o usuário ainda não tenha
    public async Task<CartHeader> CreateCartHeader(Guid userId)
    {
        CartHeader cartHeader = new CartHeader
        {
            Id = Guid.NewGuid(),
            UserId = userId
        };
        await _context.CartHeaders.AddAsync(cartHeader);
        await _context.SaveChangesAsync();
        return cartHeader;
    }
}