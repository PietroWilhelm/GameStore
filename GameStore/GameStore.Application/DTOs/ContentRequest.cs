using System.ComponentModel.DataAnnotations;

namespace GameStore.Application.DTOs;
/// <summary>
/// DTO base de requisição para conteúdo (filme, série, etc.).
/// </summary>
/// <param name="name">Título do conteúdo.</param>
/// <param name="description">Descrição do conteúdo.</param>
/// <param name="launchDate">Data de lançamento.</param>
using System.ComponentModel.DataAnnotations;

public abstract record ContentRequest(
    [Required(ErrorMessage = "O título é obrigatório")]
    [StringLength(200, MinimumLength = 2, ErrorMessage = "O título deve ter entre 2 e 200 caracteres")]
    string Name,

    [Required(ErrorMessage = "A descrição é obrigatória")]
    [StringLength(1000, MinimumLength = 10, ErrorMessage = "A descrição deve ter entre 10 e 1000 caracteres")]
    string Description,

    [Required(ErrorMessage = "A data de lançamento é obrigatória")]
    [Range(typeof(DateTime), "1958-01-01", "2100-12-31", ErrorMessage = "A data deve estar entre 1958 e 2100")]
    DateTime LaunchDate);