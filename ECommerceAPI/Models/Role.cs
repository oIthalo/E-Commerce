using System.ComponentModel.DataAnnotations;
namespace ECommerceAPI.Models;

public class Role
{
    [Key]
    public Guid Id { get; set; }

    [Required(ErrorMessage = "O nome da role é obrigatório.")]
    [StringLength(50, ErrorMessage = "O nome da role deve ter no máximo 50 caracteres.")]
    public string Name { get; set; }

    [Required(ErrorMessage = "A descrição da role é obrigatória.")]
    [StringLength(250, ErrorMessage = "A descrição da role deve ter no máximo 250 caracteres.")]
    public string Description { get; set; }
}
