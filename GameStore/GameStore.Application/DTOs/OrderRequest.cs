using System.ComponentModel.DataAnnotations;

namespace GameStore.Application.DTOs;

public record OrderRequest(
    [Required(ErrorMessage = "O identificador do cliente é obrigatório.")]
    Guid CustomerId,

    [Range(0.01, double.MaxValue, ErrorMessage = "O valor total deve ser maior que zero.")]
    decimal TotalValue,

    [Required(ErrorMessage = "A data do pedido é obrigatória.")]
    DateTime OrderDate);
