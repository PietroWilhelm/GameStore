using GameStore.Domain.Entities;

namespace GameStore.Application.DTOs;

/// <summary>
/// DTO de resposta para cliente.
/// <para>Não expõe a senha (hash) nem o salt — apenas dados seguros de exibição.</para>
/// </summary>
public record CustomerResponse(Guid Id, string Name, string Email, string Cpf, int Age)
{
    /// <summary>
    /// Mapeia <see cref="Customer"/> para DTO, sem vazar dados sensíveis.
    /// </summary>
    public static CustomerResponse FromDomain(Customer customer) =>
        new(customer.Id, customer.Name, customer.Email, customer.Cpf, customer.Age);
}
