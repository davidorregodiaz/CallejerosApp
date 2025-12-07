namespace Adoption.API.Extensions;

public static class QueryableExtensions
{
    public static IEnumerable<T> OrderByProperty<T> (this IEnumerable<T> source, string propertyName, bool descending = false)
    {
        if (string.IsNullOrWhiteSpace(propertyName))
            return source;

        var prop = typeof(T).GetProperty(propertyName);

        if (prop == null)
            return source;

        return descending
            ? source.OrderByDescending(x => prop.GetValue(x, null))
            : source.OrderBy(x => prop.GetValue(x, null));

    }
}
