using System.ComponentModel.DataAnnotations;

namespace GameStore.Application.DTOs;

public record GenreRequest(
    [Required(ErrorMessage = "O nome do gênero é obrigatório.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "O nome do gênero deve ter entre 2 e 100 caracteres.")]
    string Name,

    [Required(ErrorMessage = "A descrição do gênero é obrigatória.")]
    [StringLength(500, MinimumLength = 10, ErrorMessage = "A descrição deve ter entre 10 e 500 caracteres.")]
    string Description);
