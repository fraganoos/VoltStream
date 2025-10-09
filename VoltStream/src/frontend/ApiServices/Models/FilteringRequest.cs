namespace ApiServices.Models;

public record FilteringRequest(
    Dictionary<string, List<string>>? Filters,
    string? Search,
    int Page,
    int PageSize,
    string? SortBy,
    bool Descending);
