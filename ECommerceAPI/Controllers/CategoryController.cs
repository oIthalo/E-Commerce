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
public class CategoryController : ControllerBase
{
    private ECommerceContext _context;
    private IMapper _mapper;

    public CategoryController(ECommerceContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    /// <summary>
    /// Obtém todas as categorias no banco de dados
    /// </summary>
    /// <param name="skip">Número de categorias a pular</param>
    /// <param name="take">Número de categorias a retornar</param>
    /// <returns>Lista de categorias</returns>
    [HttpGet]
    [Authorize]
    public async Task<IEnumerable<CategoryDtoRead>> GetCategories([FromQuery] int skip = 0, [FromQuery] int take = 100)
    {
        var categories = await _context.Categories.Skip(skip).Take(take).ToListAsync();
        return _mapper.Map<IEnumerable<CategoryDtoRead>>(categories);
    }

    /// <summary>
    /// Obtém uma categoria no banco de dados pelo ID
    /// </summary>
    /// <param name="id">ID da categoria</param>
    /// <returns>IActionResult</returns>
    [HttpGet("{id}")]
    [Authorize]
    public async Task<IActionResult> GetCategoryById(Guid id)
    {
        var category = await _context.Categories.FirstOrDefaultAsync(x => x.Id.Equals(id));
        if (category is null) return NotFound(new { message = "Categoria não encontrada" });
        var categoryDto = _mapper.Map<CategoryDtoRead>(category);
        return Ok(categoryDto);
    }

    /// <summary>
    /// Adiciona uma nova categoria no banco de dados
    /// </summary>
    /// <param name="categoryDto">Dados da categoria a ser criada</param>
    /// <returns>IActionResult</returns>
    [HttpPost]
    [Authorize(Roles = "manager")]
    public async Task<IActionResult> AddCategory([FromBody] CategoryDtoCreate categoryDto)
    {
        try
        {
            Category category = _mapper.Map<Category>(categoryDto);
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();
            var categoryToRead = _mapper.Map<CategoryDtoRead>(category);
            return CreatedAtAction(nameof(GetCategoryById), new { id = categoryToRead.Id }, categoryToRead);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Atualiza todos os campos de uma categoria pelo ID
    /// </summary>
    /// <param name="id">ID da categoria</param>
    /// <param name="categoryDto">Dados atualizados da categoria</param>
    /// <returns>IActionResult</returns>
    [HttpPut("{id}")]
    [Authorize(Roles = "manager")]
    public async Task<IActionResult> UpdateCategory(Guid id, [FromBody] CategoryDtoUpdate categoryDto)
    {
        var categoryToUpdate = await _context.Categories.FirstOrDefaultAsync(x => x.Id.Equals(id));
        if (categoryToUpdate is null) return NotFound(new { message = "Categoria não encontrada" });
        _mapper.Map(categoryDto, categoryToUpdate);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    /// <summary>
    /// Atualiza um campo específico de uma categoria pelo ID
    /// </summary>
    /// <param name="id">ID da categoria</param>
    /// <param name="patch">Documento JSON Patch com as alterações</param>
    /// <returns>IActionResult</returns>
    [HttpPatch("{id}")]
    [Authorize(Roles = "manager")]
    public async Task<IActionResult> UpdateCategoryPatch(Guid id, JsonPatchDocument<CategoryDtoUpdate> patch)
    {
        var category = await _context.Categories.FirstOrDefaultAsync(x => x.Id.Equals(id));
        if (category is null) return NotFound(new { message = "Categoria não encontrada" });

        var categoryToUpdate = _mapper.Map<CategoryDtoUpdate>(category);
        patch.ApplyTo(categoryToUpdate, ModelState);

        if (!TryValidateModel(categoryToUpdate))
            return ValidationProblem(ModelState);

        _mapper.Map(categoryToUpdate, category);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    /// <summary>
    /// Exclui uma categoria pelo ID
    /// </summary>
    /// <param name="id">ID da categoria</param>
    /// <returns>IActionResult</returns>
    [HttpDelete("{id}")]
    [Authorize(Roles = "manager")]
    public async Task<IActionResult> DeleteCategory(Guid id)
    {
        var category = await _context.Categories.FirstOrDefaultAsync(x => x.Id.Equals(id));
        if (category is null) return NotFound(new { message = "Categoria não encontrada" });
        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}