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
public class RoleController : ControllerBase
{
    private ECommerceContext _context;
    private IMapper _mapper;

    public RoleController(ECommerceContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    /// <summary>
    /// Obtém todas as funções disponíveis no sistema
    /// </summary>
    /// <returns>Uma lista de funções</returns>
    [HttpGet]
    [Authorize(Roles = "manager")]
    public async Task<IEnumerable<RoleDtoRead>> GetRoles()
    {
        var roles = await _context.Roles.ToListAsync();
        return _mapper.Map<IEnumerable<RoleDtoRead>>(roles);
    }

    /// <summary>
    /// Obtém uma função pelo ID
    /// </summary>
    /// <param name="id">ID da função</param>
    /// <returns>IActionResult</returns>
    [HttpGet("{id}")]
    [Authorize(Roles = "manager")]
    public async Task<IActionResult> GetRoleById(Guid id)
    {
        var role = await _context.Roles.FirstOrDefaultAsync(x => x.Id.Equals(id));
        if (role is null) return NotFound();

        var roleDto = _mapper.Map<RoleDtoRead>(role);
        return Ok(roleDto);
    }

    /// <summary>
    /// Adiciona uma nova função
    /// </summary>
    /// <param name="roleDto">Dados da função a ser adicionada</param>
    /// <returns>IActionResult</returns>
    [HttpPost]
    [Authorize(Roles = "manager")]
    public async Task<IActionResult> AddRole(RoleDtoCreate roleDto)
    {
        Role role = _mapper.Map<Role>(roleDto);
        await _context.Roles.AddAsync(role);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetRoleById), new { id = role.Id }, role);
    }

    /// <summary>
    /// Atualiza todos os campos de uma função pelo ID
    /// </summary>
    /// <param name="id">ID da função</param>
    /// <param name="roleDto">Dados atualizados da função</param>
    /// <returns>IActionResult</returns>
    [HttpPut("{id}")]
    [Authorize(Roles = "manager")]
    public async Task<IActionResult> UpdateRole(Guid id, RoleDtoUpdate roleDto)
    {
        var roleToUpdate = await _context.Roles.FirstOrDefaultAsync(x => x.Id.Equals(id));
        if (roleToUpdate is null) return NotFound();

        _mapper.Map(roleDto, roleToUpdate);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    /// <summary>
    /// Atualiza um campo específico de uma função pelo ID
    /// </summary>
    /// <param name="id">ID da função</param>
    /// <param name="patch">Documentação do patch</param>
    /// <returns>IActionResult</returns>
    [HttpPatch("{id}")]
    [Authorize(Roles = "manager")]
    public async Task<IActionResult> UpdateRolePatch(Guid id, JsonPatchDocument<RoleDtoUpdate> patch)
    {
        var role = await _context.Roles.FirstOrDefaultAsync(x => x.Id.Equals(id));
        if (role is null) return NotFound();

        var roleToUpdate = _mapper.Map<RoleDtoUpdate>(role);
        patch.ApplyTo(roleToUpdate, ModelState);

        if (!TryValidateModel(roleToUpdate))
            return ValidationProblem(ModelState);

        _mapper.Map(roleToUpdate, role);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    /// <summary>
    /// Exclui uma função pelo ID
    /// </summary>
    /// <param name="id">ID da função</param>
    /// <returns>IActionResult</returns>
    [HttpDelete("{id}")]
    [Authorize(Roles = "manager")]
    public async Task<IActionResult> DeleteRole(Guid id)
    {
        var role = await _context.Roles.FirstOrDefaultAsync(x => x.Id.Equals(id));
        if (role is null) return NotFound();
        _context.Roles.Remove(role);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
