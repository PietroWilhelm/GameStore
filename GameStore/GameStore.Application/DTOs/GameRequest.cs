using System.ComponentModel.DataAnnotations;
using GameStore.Domain.Entities;
using GameStore.Domain.Enums;

namespace GameStore.Application.DTOs;

/// <summary>
/// DTO de requisição para criação de Games.
/// </summary>
/// <param name="Name">Nome do jogo.</param>
/// <param name="Description">Descrição do jogo.</param>
/// <param name="LaunchDate">Data de lançamento.</param>
/// <param name="StudioId">Identificador do estúdio responsável.</param>
/// <param name="ContentTypeEnum">Tipo do conteúdo.</param>
public record GameRequest(
    [Required(ErrorMessage = "O nome do jogo é obrigatório")]
    [StringLength(200, MinimumLength = 2, ErrorMessage = "O nome do jogo deve ter entre 2 e 200 caracteres")]
    string Name,

    [Required(ErrorMessage = "A descrição é obrigatória")]
    [StringLength(1000, MinimumLength = 10, ErrorMessage = "A descrição deve ter entre 10 e 1000 caracteres")]
    string Description,

    [Required(ErrorMessage = "A data de lançamento é obrigatória")]
    [Range(typeof(DateTime), "1958-01-01", "2100-12-31", ErrorMessage = "A data de lançamento deve estar entre 1958 e 2100")]
    DateTime LaunchDate,

    [Required(ErrorMessage = "O StudioId é obrigatório")]
    Guid StudioId,

    [Required(ErrorMessage = "O tipo de conteúdo é obrigatório")]
    [EnumDataType(typeof(ContentTypeEnum), ErrorMessage = "O tipo de conteúdo inválido")]
    ContentTypeEnum ContentTypeEnum
) : ContentRequest(Name, Description, LaunchDate)
{
    public Game ToDomain() => new(
        Name,
        Description,
        LaunchDate,
        ContentTypeEnum,
        StudioId);
}
