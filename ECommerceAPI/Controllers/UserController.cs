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
public class UserController : ControllerBase
{
    private ECommerceContext _context;
    private IMapper _mapper;

    public UserController(ECommerceContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    /// <summary>
    /// Obtém todos os usuários do sistema
    /// </summary>
    /// <returns>Uma lista de usuários</returns>
    [HttpGet]
    [Authorize(Roles = "manager")]
    public async Task<IEnumerable<UserDtoRead>> GetUsers()
    {
        var users = await _context.Users.ToListAsync();
        foreach (var user in users)
        {
            var role = _context.Roles.FirstOrDefault(x => x.Id.Equals(user.RoleId));
            user.Role.Name = role.Name;
        }
        return _mapper.Map<IEnumerable<UserDtoRead>>(users);
    }

    /// <summary>
    /// Obtém um usuário pelo ID
    /// </summary>
    /// <param name="id">ID do usuário</param>
    /// <returns>IActionResult</returns>
    [HttpGet("{id}")]
    [Authorize(Roles = "manager")]
    public async Task<IActionResult> GetUserById(Guid id)
    {
        var user = await _context.Users.FirstOrDefaultAsync(x => x.Id.Equals(id));
        if (user is null) return NotFound();

        var role = _context.Roles.FirstOrDefault(x => x.Id.Equals(user.RoleId));
        user.Role.Name = role.Name;

        var userDto = _mapper.Map<UserDtoRead>(user);
        return Ok(userDto);
    }

    /// <summary>
    /// Registra um novo usuário
    /// </summary>
    /// <param name="model">Dados do usuário a ser registrado</param>
    /// <returns>IActionResult</returns>
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] UserDtoRegister model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        User user = _mapper.Map<User>(model);

        var clientRole = await _context.Roles.FirstOrDefaultAsync(x => x.Name == "Client");
        if (clientRole == null)
            return NotFound();

        user.RoleId = clientRole.Id;
        user.Role = clientRole;


        try
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            CartHeader cartHeader = new CartHeader
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
            };
            await _context.CartHeaders.AddAsync(cartHeader);
            await _context.SaveChangesAsync();

            var userDto = _mapper.Map<UserDtoRead>(user);
            return Ok(userDto);
        }
        catch (Exception)
        {
            return BadRequest(new { message = "Não foi possível criar o usuário" });
        }
    }

    /// <summary>
    /// Realiza o login de um usuário
    /// </summary>
    /// <param name="model">Dados de login do usuário</param>
    /// <returns>IActionResult</returns>
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] UserDtoLogin model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var user = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(x =>
                x.Username.Equals(model.Username) &&
                x.Password.Equals(model.Password));

        if (user is null)
            return NotFound(new { message = "Nome de usuário ou senha inválidos" });

        var role = await _context.Roles.FirstOrDefaultAsync(x => x.Id.Equals(user.RoleId));
        if (role == null)
            return NotFound();
        user.Role = role;

        var token = TokenService.GenerateToken(user);
        return Ok(new
        {
            user = user.Username,
            role = role.Name,
            token = token,
        });
    }

    /// <summary>
    /// Atualiza todos os campos de um usuário pelo ID
    /// </summary>
    /// <param name="id">ID do usuário</param>
    /// <param name="model">Dados atualizados do usuário</param>
    /// <returns>IActionResult</returns>
    [HttpPut("{id}")]
    [Authorize(Roles = "manager")]
    public async Task<IActionResult> UpdateUser(Guid id, UserDtoUpdate model)
    {
        var userToUpdate = await _context.Users.FirstOrDefaultAsync(x => x.Id.Equals(id));
        if (userToUpdate is null) return NotFound();
        _mapper.Map(model, userToUpdate);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    /// <summary>
    /// Atualiza um campo específico de um usuário pelo ID
    /// </summary>
    /// <param name="id">ID do usuário</param>
    /// <param name="patch">Documentação do patch</param>
    /// <returns>IActionResult</returns>
    [HttpPatch("{id}")]
    [Authorize(Roles = "manager")]
    public async Task<IActionResult> UpdateUserPatch(Guid id, JsonPatchDocument<UserDtoUpdate> patch)
    {
        var user = await _context.Users.FirstOrDefaultAsync(x => x.Id.Equals(id));
        if (user is null) return NotFound();

        var userToUpdate = _mapper.Map<UserDtoUpdate>(user);
        patch.ApplyTo(userToUpdate, ModelState);

        if (!TryValidateModel(userToUpdate))
            return ValidationProblem(ModelState);

        _mapper.Map(userToUpdate, user);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    /// <summary>
    /// Exclui um usuário pelo ID
    /// </summary>
    /// <param name="id">ID do usuário</param>
    /// <returns>IActionResult</returns>
    [HttpDelete("{id}")]
    [Authorize(Roles = "manager")]
    public async Task<IActionResult> DeleteUser(Guid id)
    {
        var user = await _context.Users.FirstOrDefaultAsync(x => x.Id.Equals(id));
        if (user is null) return NotFound();

        var cartHeader = await _context.CartHeaders.FirstOrDefaultAsync(x => x.UserId.Equals(user.Id));

        _context.Users.Remove(user);
        _context.CartHeaders.Remove(cartHeader);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
