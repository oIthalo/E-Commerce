using System.ComponentModel.DataAnnotations;
namespace ECommerceAPI.Models;

public class CartItem
{
    [Key]
    [Required]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required(ErrorMessage = "A quantidade é obrigatória.")]
    [Range(1, int.MaxValue, ErrorMessage = "A quantidade deve ser pelo menos 1.")]
    public int Quantity { get; set; }

    [Required(ErrorMessage = "O ID do produto é obrigatório.")]
    public Guid ProductId { get; set; }

    [Required(ErrorMessage = "O ID do cabeçalho do carrinho é obrigatório.")]
    public Guid CartHeaderId { get; set; }

    [Required(ErrorMessage = "O produto é obrigatório.")]
    public virtual Product Product { get; set; }

    [Required(ErrorMessage = "O cabeçalho do carrinho é obrigatório.")]
    public virtual CartHeader CartHeader { get; set; }
}