using System.ComponentModel.DataAnnotations;
namespace ECommerceAPI.Models;

public class CartHeader
{
    [Key]
    [Required]
    public Guid Id { get; set; }  = Guid.NewGuid();
    [Required(ErrorMessage = "Id do usuário é obrigatório")]
    public Guid UserId { get; set; }
}