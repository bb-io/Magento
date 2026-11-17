using System.Globalization;
using RestSharp;

namespace Apps.Magento.Utils;

public class SearchCriteriaBuilder
{
    private readonly List<List<(string Field, string Value, string Condition)>> _groups = [];

    public SearchCriteriaBuilder WhereEqual(params (string Field, object? Value)[] filters) => AddGroup("eq", filters);
    public SearchCriteriaBuilder WhereNotEqual(params (string Field, object? Value)[] filters) => AddGroup("neq", filters);

    public SearchCriteriaBuilder WhereGreaterThan(params (string Field, object? Value)[] filters) => AddGroup("gt", filters);
    public SearchCriteriaBuilder WhereGreaterThanOrEqual(params (string Field, object? Value)[] filters) => AddGroup("gteq", filters);
    public SearchCriteriaBuilder WhereLessThan(params (string Field, object? Value)[] filters) => AddGroup("lt", filters);
    public SearchCriteriaBuilder WhereLessThanOrEqual(params (string Field, object? Value)[] filters) => AddGroup("lteq", filters);

    public SearchCriteriaBuilder WhereLike(params (string Field, string? Pattern)[] filters) =>
        AddGroup("like", filters.Select(f => (f.Field, (object?)f.Pattern)));

    public SearchCriteriaBuilder WhereNotLike(params (string Field, string? Pattern)[] filters) =>
        AddGroup("nlike", filters.Select(f => (f.Field, (object?)f.Pattern)));

    public SearchCriteriaBuilder WhereContains(params (string Field, string? Value)[] filters) =>
        WhereLike(filters.Select(f => (f.Field, string.IsNullOrEmpty(f.Value) ? null : $"%{f.Value}%")).ToArray());

    public SearchCriteriaBuilder WhereIn(params (string Field, IEnumerable<object>? Values)[] filters) =>
        AddGroup("in", filters.Select(f => (f.Field, (object?)f.Values)));

    public SearchCriteriaBuilder WhereNotIn(params (string Field, IEnumerable<object>? Values)[] filters) =>
        AddGroup("nin", filters.Select(f => (f.Field, (object?)f.Values)));

    public RestRequest ApplyTo(RestRequest request)
    {
        for (var i = 0; i < _groups.Count; i++)
        for (var j = 0; j < _groups[i].Count; j++)
        {
            var (field, value, condition) = _groups[i][j];
            var prefix = $"searchCriteria[filterGroups][{i}][filters][{j}]";

            request.AddQueryParameter($"{prefix}[field]", field)
                .AddQueryParameter($"{prefix}[value]", value)
                .AddQueryParameter($"{prefix}[conditionType]", condition);
        }

        return request;
    }

    private SearchCriteriaBuilder AddGroup(string condition, IEnumerable<(string Field, object? Value)> filters)
    {
        var group = filters
            .Select(f => (f.Field, Value: Format(f.Value)))
            .Where(f => !string.IsNullOrEmpty(f.Value))
            .Select(f => (f.Field, f.Value!, condition))
            .ToList();

        if (group.Count > 0)
            _groups.Add(group);

        return this;
    }

    private static string? Format(object? value) => value switch
    {
        null => null,
        string text => text,
        DateTime date => date.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture),
        IFormattable formattable => formattable.ToString(null, CultureInfo.InvariantCulture),
        IEnumerable<object> values => string.Join(",", values.Select(Format).Where(x => !string.IsNullOrEmpty(x))),
        _ => value.ToString()
    };
}