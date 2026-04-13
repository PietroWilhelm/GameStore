using System.ComponentModel.DataAnnotations;
using GameStore.Domain.Entities;
using GameStore.Domain.Enum;

namespace GameStore.Application.DTOs;

/// <summary>
/// DTO de requisição para criação de Games.
/// </summary>
/// <param name="Name">nome do jogo.</param>
/// <param name="Description">Descrição do jogo.</param>
/// <param name="launchDate">Data de lançamento.</param>
/// <param name="contentTypeEnum">Tipo do conteúdo.</param>
public record GameRequest(
    string Name,
    string Description,
    DateTime launchDate,
    Guid StudioId,
    
    [property:Required(ErrorMessage = "O tipo de conteúdo é obrigatório")]
    [property:EnumDataType(typeof(ContentTypeEnum), ErrorMessage = "O tipo de conteúdo invalido")]
    ContentTypeEnum contentTypeEnum
    ) : ContentRequest (Name, Description, launchDate)
{
    // poderia ser feito via autoMapper, conteudo ele e pago entao como feito em sala, vai ser na mao
    
    public Game toDomain() => new Game(
        Name, 
        Description,
        launchDate,
        contentTypeEnum,
        StudioId);
}