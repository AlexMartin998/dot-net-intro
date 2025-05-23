using Microsoft.EntityFrameworkCore;

public class PaginationFilter
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SortBy { get; set; }
    public bool SortDescending { get; set; }
    public string? SearchQuery { get; set; }
    
    public Dictionary<string, string> Filters { get; set; } = new();
}

public static class QueryExtensions
{
    public static IQueryable<T> ApplyFilter<T>(this IQueryable<T> query, PaginationFilter filter)
    {
        if (!string.IsNullOrEmpty(filter.SearchQuery))
        {
            var searchTerm = $"%{filter.SearchQuery.ToLower()}%";
            query = query.Where(x => EF.Functions.Like((EF.Property<string>(x!, "Name") ?? "").ToLower(), searchTerm));
        }

        foreach (var (key, value) in filter.Filters)
        {
            var property = typeof(T).GetProperty(key);
            if (property != null)
            {
                query = query.Where(x => x != null && (EF.Property<string>(x, key) ?? "").Contains(value));
            }
        }

        if (!string.IsNullOrEmpty(filter.SortBy))
        {
            query = filter.SortDescending 
                ? query.OrderByDescending(x => EF.Property<object>(x!, filter.SortBy))
                : query.OrderBy(x => EF.Property<object>(x!, filter.SortBy));
        }

        return query
            .Skip((filter.PageNumber - 1) * filter.PageSize)
            .Take(filter.PageSize);
    }
}