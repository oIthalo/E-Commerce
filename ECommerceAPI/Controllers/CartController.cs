using AutoMapper;
using ECommerceAPI.Data;
using ECommerceAPI.Data.DTOs;
using ECommerceAPI.Models;
using ECommerceAPI.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ECommerceAPI.Controllers;

[ApiController]
[Route("v1/[controller]")]
public class CartController : ControllerBase
{
    private ECommerceContext _context;
    private IMapper _mapper;
    private CartService _cartService;

    public CartController(ECommerceContext context, IMapper mapper, CartService cartService)
    {
        _context = context;
        _mapper = mapper;
        _cartService = cartService;
    }

    /// <summary>
    /// Obtém todos os cabeçalhos de carrinhos com paginação.
    /// Requer autenticação com a role de "manager".
    /// </summary>
    /// <param name="skip">Número de registros a pular (default: 0)</param>
    /// <param name="take">Número máximo de registros a retornar (default: 300)</param>
    /// <returns>Uma lista de cabeçalhos de carrinhos</returns>
    [HttpGet]
    [Authorize(Roles = "manager")]
    public async Task<IEnumerable<CartHeader>> GetCartsHeaders([FromQuery] int skip = 0, [FromQuery] int take = 300)
    {
        var cartHeaders = _context.CartHeaders.Skip(skip).Take(take);
        return cartHeaders.ToList();
    }

    /// <summary>
    /// Obtém o cabeçalho do carrinho de um usuário específico.
    /// Requer autenticação com a role de "manager".
    /// </summary>
    /// <param name="userId">ID do usuário</param>
    /// <returns>O cabeçalho do carrinho associado ao usuário</returns>
    [HttpGet("{userId}")]
    [Authorize(Roles = "manager")]
    public async Task<IActionResult> GetCartHeaderByUserId(Guid userId)
    {
        var cartHeader = await _cartService.GetCartByUserId(userId);
        if (cartHeader is null)
            return NotFound();
        return Ok(cartHeader);
    }

    /// <summary>
    /// Obtém todos os itens de carrinho de um usuário específico.
    /// Requer autenticação com a role de "manager".
    /// </summary>
    /// <param name="userId">ID do usuário</param>
    /// <returns>Uma lista de itens de carrinho do usuário</returns>
    [HttpGet("CartItems/{userId}")]
    [Authorize(Roles = "manager")]
    public async Task<IEnumerable<CartItemDto>> GetCartItemsByUserId(Guid userId)
    {
        var cartItems = await _context.CartItems
            .Where(ci => ci.CartHeader.UserId == userId)
            .ToListAsync();

        var cartItemsDto = _mapper.Map<IEnumerable<CartItemDto>>(cartItems);

        foreach (var cartItemDto in cartItemsDto)
        {
            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.Id == cartItemDto.ProductId);

            if (product != null)
                cartItemDto.ProductName = product.Name;
        }

        return cartItemsDto;
    }

    /// <summary>
    /// Adiciona um novo item ao carrinho do usuário.
    /// Requer autenticação.
    /// </summary>
    /// <param name="cartItemDto">Dados do item a ser adicionado</param>
    /// <returns>O item adicionado com o nome do produto</returns>
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> AddCartItem(CartItemDtoCreate cartItemDto)
    {
        var product = await _context.Products
            .FirstOrDefaultAsync(x => x.Id.Equals(cartItemDto.ProductId));

        if (product == null)
            return NotFound("Produto não encontrado");

        var cartItem = new CartItemDtoRead
        {
            ProductName = product.Name,
            ProductId = cartItemDto.ProductId,
            Quantity = cartItemDto.Quantity,
            CartHeaderId = cartItemDto.CartHeaderId,
        };

        await _cartService.AddCartItem(cartItemDto);
        return Ok(cartItem);
    }

    /// <summary>
    /// Atualiza um item do carrinho.
    /// Requer autenticação.
    /// </summary>
    /// <param name="id">ID do item do carrinho a ser atualizado</param>
    /// <param name="cartDto">Dados atualizados do item</param>
    /// <returns>NoContent</returns>
    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> UpdateCartItem(Guid id, CartItemDtoUpdate cartDto)
    {
        await _cartService.UpdateCartItem(id, cartDto);
        return NoContent();
    }

    /// <summary>
    /// Atualiza parcialmente um item do carrinho.
    /// Requer autenticação.
    /// </summary>
    /// <param name="itemId">ID do item do carrinho a ser atualizado</param>
    /// <param name="patch">Operação de patch contendo as atualizações</param>
    /// <returns>NoContent</returns>
    [HttpPatch("{id}")]
    [Authorize]
    public async Task<IActionResult> UpdateCartItemPatch(Guid itemId, JsonPatchDocument<CartItemDtoUpdate> patch)
    {
        await _cartService.PatchCartItem(itemId, patch);
        return NoContent();
    }

    /// <summary>
    /// Remove um item do carrinho.
    /// Requer autenticação.
    /// </summary>
    /// <param name="itemId">ID do item do carrinho a ser removido</param>
    /// <returns>NoContent</returns>
    [HttpDelete("Clear/{itemId}")]
    [Authorize]
    public async Task<IActionResult> RemoveCartItem(Guid itemId)
    {
        await _cartService.RemoveCartItem(itemId);
        return NoContent();
    }

    /// <summary>
    /// Remove todos os itens do carrinho associado ao cabeçalho de carrinho especificado.
    /// Requer autenticação com a role de "manager".
    /// </summary>
    /// <param name="cartHeaderId">ID do cabeçalho do carrinho</param>
    /// <returns>NoContent</returns>
    [HttpDelete("RemoveCart/{cartHeaderId}")]
    [Authorize(Roles = "manager")]
    public async Task<IActionResult> ClearByCartId(Guid cartHeaderId)
    {
        await _cartService.ClearByCartId(cartHeaderId);
        return NoContent();
    }
}
