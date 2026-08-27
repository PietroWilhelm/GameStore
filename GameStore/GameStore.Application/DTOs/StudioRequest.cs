using System.ComponentModel.DataAnnotations;

namespace GameStore.Application.DTOs;

public record StudioRequest(
    [Required(ErrorMessage = "O nome da desenvolvedora é obrigatório.")]
    [StringLength(150, MinimumLength = 2, ErrorMessage = "O nome deve ter entre 2 e 150 caracteres.")]
    string Name,

    [Required(ErrorMessage = "A data de fundação é obrigatória.")]
    DateTime FoundationDate);
