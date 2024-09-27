using System.ComponentModel.DataAnnotations;
namespace ECommerceAPI.Models;

public class Category
{
    [Key]
    [Required]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required(ErrorMessage = "O nome da categoria é obrigatório.")]
    [StringLength(100, ErrorMessage = "O nome da categoria deve ter no máximo 100 caracteres.")]
    public string Name { get; set; }

    [Required(ErrorMessage = "A descrição da categoria é obrigatória.")]
    [StringLength(500, ErrorMessage = "A descrição da categoria deve ter no máximo 500 caracteres.")]
    public string Description { get; set; }

    [Required(ErrorMessage = "O slug é obrigatório.")]
    [StringLength(100, ErrorMessage = "O slug deve ter no máximo 100 caracteres.")]
    [RegularExpression(@"^[a-z0-9]+(?:-[a-z0-9]+)*$", ErrorMessage = "O slug deve conter apenas letras minúsculas, números e hífens.")]
    public string Slug { get; set; }
}