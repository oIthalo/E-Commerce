using System.ComponentModel.DataAnnotations;
namespace ECommerceAPI.Models;

public class Product
{
    [Key]
    [Required]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required(ErrorMessage = "O nome do produto é obrigatório.")]
    [StringLength(100, ErrorMessage = "O nome do produto deve ter no máximo 100 caracteres.")]
    public string Name { get; set; }

    [Required(ErrorMessage = "A descrição do produto é obrigatória.")]
    [StringLength(500, ErrorMessage = "A descrição do produto deve ter no máximo 500 caracteres.")]
    public string Description { get; set; }

    [Required(ErrorMessage = "O produto precisa de uma categoria.")]
    public Guid CategoryId { get; set; }

    [Required(ErrorMessage = "O produto precisa de uma categoria.")]
    public virtual Category? Category { get; set; }

    [Required(ErrorMessage = "O preço é obrigatório.")]
    [Range(0.01, double.MaxValue, ErrorMessage = "O preço deve ser maior que zero.")]
    public double Price { get; set; }

    [Required(ErrorMessage = "O estoque é obrigatório.")]
    [Range(0, int.MaxValue, ErrorMessage = "O estoque não pode ser negativo.")]
    public int Stock { get; set; }

    [Required(ErrorMessage = "A URL da imagem é obrigatória.")]
    [Url(ErrorMessage = "A URL da imagem é inválida.")]
    public string ImageUrl { get; set; }

    [Required(ErrorMessage = "A data de publicação é obrigatória.")]
    public DateTime PublicationDate { get; set; } = DateTime.Now;
}