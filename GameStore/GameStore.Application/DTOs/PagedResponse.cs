namespace GameStore.Application.DTOs;

/// <summary>
/// Envelope de paginação genérico retornado pelas listagens paginadas (contrato v2.0).
/// Os nomes de campo (page, pageSize, totalItems, totalPages, items) fazem parte do contrato
/// e não devem ser renomeados.
/// </summary>
/// <typeparam name="T">Tipo do item listado.</typeparam>
/// <param name="Page">Página atual (1-based).</param>
/// <param name="PageSize">Quantidade de itens por página.</param>
/// <param name="TotalItems">Total de itens existentes, sem considerar a paginação.</param>
/// <param name="TotalPages">Total de páginas: ceiling(TotalItems / PageSize).</param>
/// <param name="Items">Itens pertencentes à página atual.</param>
public record PagedResponse<T>(
    int Page,
    int PageSize,
    int TotalItems,
    int TotalPages,
    IReadOnlyList<T> Items)
{
    /// <summary>Indica se existe uma página anterior.</summary>
    public bool HasPrevious => Page > 1;

    /// <summary>Indica se existe uma próxima página.</summary>
    public bool HasNext => Page < TotalPages;

    /// <summary>
    /// Constrói o envelope de paginação calculando <see cref="TotalPages"/> a partir do total de itens.
    /// </summary>
    public static PagedResponse<T> Create(IReadOnlyList<T> items, int page, int pageSize, int totalItems)
    {
        var totalPages = totalItems == 0
            ? 0
            : (int)Math.Ceiling(totalItems / (double)pageSize);

        return new PagedResponse<T>(page, pageSize, totalItems, totalPages, items);
    }
}
