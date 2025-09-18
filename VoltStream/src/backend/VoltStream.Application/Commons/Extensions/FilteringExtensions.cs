namespace VoltStream.Application.Commons.Extensions;

using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using VoltStream.Application.Commons.Exceptions;
using VoltStream.Application.Commons.Models;

public static class FilteringExtensions
{
    public static IQueryable<T> AsFilterable<T>(this IQueryable<T> query, FilteringRequest request)
    {
        var param = Expression.Parameter(typeof(T), "x");
        var props = typeof(T).GetProperties();

        foreach (var entry in request.Filters ?? [])
        {
            var prop = props.FirstOrDefault(p => string.Equals(p.Name, entry.Key, StringComparison.OrdinalIgnoreCase));
            if (prop is null) continue;

            var member = Expression.Property(param, prop.Name);
            try
            {
                var convertedValue = ConversionHelper.TryConvert(entry.Value, prop.PropertyType);
                var constant = Expression.Constant(convertedValue, prop.PropertyType);
                var body = Expression.Equal(member, constant);
                var lambda = Expression.Lambda<Func<T, bool>>(body, param);
                query = query.Where(lambda);
            }
            catch (Exception ex)
            {
                throw new AppException($"'{entry.Key}' bo‘yicha filter qiymatini o‘zgartirishda xatolik: {ex.Message}");
            }
        }

        // 🔎 Global search (case-insensitive)
        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var stringProps = typeof(T).GetProperties().Where(p => p.PropertyType == typeof(string));
            Expression? searchExpr = null;

            foreach (var p in stringProps)
            {
                var member = Expression.Property(param, p.Name);

                var notNull = Expression.NotEqual(member, Expression.Constant(null, typeof(string)));
                var memberToLower = Expression.Call(member, nameof(string.ToLower), Type.EmptyTypes);
                var searchValue = Expression.Constant(request.Search.ToLower());
                var contains = Expression.Call(memberToLower, nameof(string.Contains), Type.EmptyTypes, searchValue);

                var condition = Expression.AndAlso(notNull, contains);
                searchExpr = searchExpr is null ? condition : Expression.OrElse(searchExpr, condition);
            }

            if (searchExpr is not null)
            {
                var lambda = Expression.Lambda<Func<T, bool>>(searchExpr, param);
                query = query.Where(lambda);
            }
        }

        return query.AsSortable(request).AsPagable(request);
    }

    public static async Task<PagedList<T>> ToPagedListAsync<T>(
        this IQueryable<T> query,
        FilteringRequest request,
        CancellationToken cancellationToken = default)
    {
        var filtered = query.AsFilterable(request);
        var total = await filtered.CountAsync(cancellationToken);
        var items = await filtered.ToListAsync(cancellationToken);

        return new PagedList<T>(items, total, request.Page, request.PageSize);
    }
}