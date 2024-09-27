using ECommerceAPI.Data.DTOs;
using ECommerceAPI.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceAPI.Controllers;

[ApiController]
[Route("v1/[controller]")]
public class CartController : ControllerBase
{
    private ICartService _cartService;

    public CartController(ICartService cartService)
        => _cartService = cartService;

    /// <summary>
    /// Obtém o carrinho de um usuário pelo ID
    /// </summary>
    /// <param name="id">ID do usuário</param>
    /// <returns>IActionResult</returns>
    [HttpGet("{id}")]
    [Authorize(Roles = "manager")]
    public async Task<IActionResult> GetCartUserById(Guid id)
    {
        var cartDto = await _cartService.GetCartByUserIdAsync(id);
        if (cartDto is null) return NotFound();
        return Ok(cartDto);
    }

    /// <summary>
    /// Adiciona ou atualiza o carrinho
    /// </summary>
    /// <param name="cartDto">Dados do carrinho a ser adicionado ou atualizado</param>
    /// <returns>IActionResult</returns>
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> AddCart(CartDto cartDto)
    {
        var cart = await _cartService.UpdateCartAsync(cartDto);
        if (cart is null) return NotFound();
        return Ok(cart);
    }

    /// <summary>
    /// Atualiza o carrinho
    /// </summary>
    /// <param name="id">ID do carrinho</param>
    /// <param name="cartDto">Dados atualizados do carrinho</param>
    /// <returns>ActionResult</returns>
    [HttpPut("{id}")]
    [Authorize]
    public async Task<ActionResult<CartDto>> UpdateCart(CartDto cartDto)
    {
        var cart = await _cartService.UpdateCartAsync(cartDto);
        if (cart is null) return NotFound();
        return Ok(cart);
    }

    /// <summary>
    /// Exclui o carrinho pelo ID
    /// </summary>
    /// <param name="id">ID do carrinho</param>
    /// <returns>ActionResult</returns>
    [HttpDelete("{id}")]
    [Authorize]
    public async Task<ActionResult<bool>> DeleteCart(Guid id)
    {
        var status = await _cartService.DeleteItemCartAsync(id);
        if (!status) return BadRequest();
        return Ok(status);
    }
}
