using AutoMapper;
using ECommerceAPI.Data;
using ECommerceAPI.Data.DTOs;
using ECommerceAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ECommerceAPI.Controllers;

[ApiController]
[Route("v1/[controller]")]
public class ProductController : ControllerBase
{
    private ECommerceContext _context;
    private IMapper _mapper;

    public ProductController(ECommerceContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    /// <summary>
    /// Obtém todos os produtos no banco de dados
    /// </summary>
    /// <param name="skip">Número de produtos a pular</param>
    /// <param name="take">Número de produtos a retornar</param>
    /// <returns>Lista de produtos</returns>
    [HttpGet]
    [AllowAnonymous]
    public async Task<IEnumerable<ProductDtoRead>> GetProducts([FromQuery] int skip = 0, [FromQuery] int take = 100)
    {
        var products = await _context.Products.Include(x => x.Category).Skip(skip).Take(take).ToListAsync();
        return _mapper.Map<List<ProductDtoRead>>(products);
    }

    /// <summary>
    /// Obtém um produto no banco de dados pelo ID
    /// </summary>
    /// <param name="id">ID do produto</param>
    /// <returns>IActionResult</returns>
    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetProductById(Guid id)
    {
        var product = await _context.Products.Include(x => x.Category).FirstOrDefaultAsync(x => x.Id.Equals(id));
        if (product is null) return NotFound(new { message = "Produto não encontrado" });
        return Ok(_mapper.Map<ProductDtoRead>(product));
    }

    /// <summary>
    /// Adiciona um novo produto no banco de dados
    /// </summary>
    /// <param name="productDto">Dados do produto a ser criado</param>
    /// <returns>IActionResult</returns>
    [HttpPost]
    [Authorize(Roles = "seller")]
    public async Task<IActionResult> AddProduct([FromBody] ProductDtoCreate productDto)
    {
        Product product = _mapper.Map<Product>(productDto);
        await _context.Products.AddAsync(product);
        await _context.SaveChangesAsync();

        var createdProduct = await _context.Products
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == product.Id);

        var productResponse = _mapper.Map<ProductDtoRead>(createdProduct);

        return CreatedAtAction(nameof(GetProductById), new { id = product.Id }, productResponse);
    }

    /// <summary>
    /// Atualiza todos os campos de um produto pelo ID
    /// </summary>
    /// <param name="id">ID do produto</param>
    /// <param name="productDto">Dados atualizados do produto</param>
    /// <returns>IActionResult</returns>
    [HttpPut("{id}")]
    [Authorize(Roles = "seller")]
    public async Task<IActionResult> UpdateProduct(Guid id, ProductDtoUpdate productDto)
    {
        var productToUpdate = await _context.Products.Include(x => x.Category).FirstOrDefaultAsync(x => x.Id.Equals(id));
        if (productToUpdate is null) return NotFound(new { message = "Produto não encontrado" });

        _mapper.Map(productDto, productToUpdate);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    /// <summary>
    /// Atualiza um campo específico de um produto pelo ID
    /// </summary>
    /// <param name="id">ID do produto</param>
    /// <param name="patch">Documento JSON Patch com as alterações</param>
    /// <returns>IActionResult</returns>
    [HttpPatch("{id}")]
    [Authorize(Roles = "seller")]
    public async Task<IActionResult> UpdateProductPatch(Guid id, JsonPatchDocument<ProductDtoUpdate> patch)
    {
        var product = await _context.Products.Include(x => x.Category).FirstOrDefaultAsync(x => x.Id.Equals(id));
        if (product is null) return NotFound(new { message = "Produto não encontrado" });

        var productToUpdate = _mapper.Map<ProductDtoUpdate>(product);
        patch.ApplyTo(productToUpdate, ModelState);

        if (!TryValidateModel(productToUpdate))
            return ValidationProblem(ModelState);

        _mapper.Map(productToUpdate, product);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    /// <summary>
    /// Exclui um produto pelo ID
    /// </summary>
    /// <param name="id">ID do produto</param>
    /// <returns>IActionResult</returns>
    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> DeleteProduct(Guid id)
    {
        var product = await _context.Products.Include(x => x.Category).FirstOrDefaultAsync(x => x.Id.Equals(id));
        if (product is null) return NotFound(new { message = "Produto não encontrado" });
        _context.Products.Remove(product);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
