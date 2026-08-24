namespace GameStore.Domain.Enum;


/// <summary>
/// Subtipo de <see cref="Entities.Content"/> (espelha o discriminator TPH e o Kind da API).
/// Para adicionar um formato novo:
/// 1) incluir valor neste enum;
/// 2) criar a entidade derivada;
/// 3) criar uma <c>IContentFactory</c> concreta;
/// 4) registrar a factory no DI;
/// 5) registrar o valor em <c>HasDiscriminator</c> no EF.
/// </summary>
public enum ContentTypeEnum
{
    action,
    adventure,
    estategy,
    RPG,
    simulation,
    esports,
    running,
    Terror,
    Puzzle,
    survival,
    platform,
    fighting,
    others
}