using CollectorsApp.Data;
using CollectorsApp.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;

namespace CollectorsApp.Repository
{
    /// <summary>
    /// Generic reflective query repository. Builds a predicate from non-null DTO properties
    /// plus supports Before / After (date range), ordering and paging.
    /// </summary>
    public class QueryRepository<T, TDTO> : GenericRepository<T>, IQueryRepository<T, TDTO>
        where T : class
        where TDTO : class
    {
        private static readonly HashSet<string> ReservedDtoMembers = new(StringComparer.OrdinalIgnoreCase)
        {
            "Before",
            "After",
            "OrderBy",
            "SortDescending",
            "Page",
            "NumberOfRecords",
            "TimeStamp",
            "SearchText",
            "Keywords"
        };

        private static readonly string[] TemporalPropertyCandidates =
        {
            "TimeStamp",
            "Timestamp",
            "InsertDate",
            "AccountCreationDate",
            "DateOfIssue"
        };

        public QueryRepository(appDatabaseContext context) : base(context) { }

        public virtual async Task<IEnumerable<T>> QueryEntity(TDTO entity)
        {
            return await QueryEntity(entity, CancellationToken.None);
        }

        public virtual async Task<List<T>> QueryEntity(TDTO entity, CancellationToken cancellationToken)
        {
            IQueryable<T> query = _dbSet;

            if (entity == null)
                return await query.ToListAsync(cancellationToken);

            var dtoProps = typeof(TDTO).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var entityProps = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                       .ToDictionary(p => p.Name, p => p, StringComparer.OrdinalIgnoreCase);

            var param = Expression.Parameter(typeof(T), "e");
            Expression? predicate = null;

            // Regular property filters
            foreach (var dtoProp in dtoProps)
            {
                if (ReservedDtoMembers.Contains(dtoProp.Name))
                    continue;

                var value = dtoProp.GetValue(entity);
                if (value == null) continue;

                if (!entityProps.TryGetValue(dtoProp.Name, out var entProp))
                    continue;

                if (entProp.PropertyType == typeof(string) && value is string s && string.IsNullOrWhiteSpace(s))
                    continue;

                var left = Expression.Property(param, entProp);
                Expression condition;

                if (entProp.PropertyType == typeof(string))
                {
                    var toLowerMethod = typeof(string).GetMethod(nameof(string.ToLowerInvariant), Type.EmptyTypes)!;
                    var containsMethod = typeof(string).GetMethod(nameof(string.Contains), new[] { typeof(string) })!;

                    // left != null && leftLower.Contains(rightLower)
                    var leftLower = Expression.Call(left, toLowerMethod);
                    var rightLower = Expression.Constant(((string)value).ToLowerInvariant(), typeof(string));

                    condition = Expression.Call(leftLower, containsMethod, rightLower);
                }
                else
                {
                    // Build a correctly typed constant (handles Nullable<T>, enums, Guid, numerics)
                    var right = BuildTypedConstant(value, entProp.PropertyType);
                    if (right == null)
                        continue; // skip if we couldn't convert

                    condition = Expression.Equal(left, right);
                }

                predicate = predicate == null ? condition : Expression.AndAlso(predicate, condition);
            }

            // Phrase search: SearchText
            var searchProp = typeof(TDTO).GetProperty("SearchText", BindingFlags.Public | BindingFlags.Instance);
            if (searchProp?.GetValue(entity) is string searchText && !string.IsNullOrWhiteSpace(searchText))
            {
                Expression? searchPredicate = null;
                var toLowerMethod = typeof(string).GetMethod(nameof(string.ToLowerInvariant), Type.EmptyTypes)!;
                var containsMethod = typeof(string).GetMethod(nameof(string.Contains), new[] { typeof(string) })!;

                foreach (var stringProp in entityProps.Values.Where(p => p.PropertyType == typeof(string)))
                {
                    var left = Expression.Property(param, stringProp);
                    var leftLower = Expression.Call(left, toLowerMethod);
                    var rightLower = Expression.Constant(searchText.ToLowerInvariant());

                    var call = Expression.Call(leftLower, containsMethod, rightLower);

                    searchPredicate = searchPredicate == null ? call : Expression.OrElse(searchPredicate, call);
                }

                if (searchPredicate != null)
                    predicate = predicate == null ? searchPredicate : Expression.AndAlso(predicate, searchPredicate);
            }

            // Multi-word search: Keywords (split string into words)
            var keywordsProp = typeof(TDTO).GetProperty("Keywords", BindingFlags.Public | BindingFlags.Instance);
            if (keywordsProp?.GetValue(entity) is string keywords && !string.IsNullOrWhiteSpace(keywords))
            {
                var wordList = keywords.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                if (wordList.Length > 0)
                {
                    var toLowerMethod = typeof(string).GetMethod(nameof(string.ToLowerInvariant), Type.EmptyTypes)!;
                    var containsMethod = typeof(string).GetMethod(nameof(string.Contains), new[] { typeof(string) })!;

                    Expression? allWordsPredicate = null;

                    foreach (var word in wordList)
                    {
                        Expression? wordPredicate = null;

                        foreach (var stringProp in entityProps.Values.Where(p => p.PropertyType == typeof(string)))
                        {
                            var left = Expression.Property(param, stringProp);
                            var leftLower = Expression.Call(left, toLowerMethod);
                            var rightLower = Expression.Constant(word.ToLowerInvariant());

                            var call = Expression.Call(leftLower, containsMethod, rightLower);

                            wordPredicate = wordPredicate == null ? call : Expression.OrElse(wordPredicate, call);
                        }

                        if (wordPredicate != null)
                        {
                            allWordsPredicate = allWordsPredicate == null
                                ? wordPredicate
                                : Expression.AndAlso(allWordsPredicate, wordPredicate);
                        }
                    }

                    if (allWordsPredicate != null)
                        predicate = predicate == null ? allWordsPredicate : Expression.AndAlso(predicate, allWordsPredicate);
                }
            }

            // Apply predicate
            if (predicate != null)
            {
                var lambda = Expression.Lambda<Func<T, bool>>(predicate, param);
                query = query.Where(lambda);
            }

            // Date range filters (Before / After) against first matching temporal property
            var beforeProp = typeof(TDTO).GetProperty("Before", BindingFlags.Public | BindingFlags.Instance);
            var afterProp = typeof(TDTO).GetProperty("After", BindingFlags.Public | BindingFlags.Instance);

            var temporalProp = ResolveTemporalProperty(entityProps);
            if (temporalProp != null)
            {
                if (beforeProp?.GetValue(entity) is DateTime beforeValue)
                    query = ApplyDateFilter(query, temporalProp, beforeValue, isBefore: true);

                if (afterProp?.GetValue(entity) is DateTime afterValue)
                    query = ApplyDateFilter(query, temporalProp, afterValue, isBefore: false);
            }

            // Ordering
            string orderBy = dtoProps.FirstOrDefault(p => p.Name.Equals("OrderBy", StringComparison.OrdinalIgnoreCase))
                                ?.GetValue(entity)?.ToString() ?? "Id";

            bool sortDescending = dtoProps.FirstOrDefault(p => p.Name.Equals("SortDescending", StringComparison.OrdinalIgnoreCase))
                                ?.GetValue(entity) as bool? ?? false;

            if (!string.IsNullOrWhiteSpace(orderBy) && entityProps.TryGetValue(orderBy, out var orderProp))
            {
                query = ApplyOrdering(query, param, orderProp, sortDescending);
            }
            else if (entityProps.TryGetValue("Id", out var idProp))
            {
                query = ApplyOrdering(query, param, idProp, false);
            }
            else
            {
                var firstProp = entityProps.Values.FirstOrDefault();
                if (firstProp != null)
                    query = ApplyOrdering(query, param, firstProp, false);
            }

            // Paging
            int page = dtoProps.FirstOrDefault(p => p.Name.Equals("Page", StringComparison.OrdinalIgnoreCase))
                        ?.GetValue(entity) as int? ?? 1;
            int pageSize = dtoProps.FirstOrDefault(p => p.Name.Equals("NumberOfRecords", StringComparison.OrdinalIgnoreCase))
                        ?.GetValue(entity) as int? ?? 10;

            page = Math.Max(page, 1);
            pageSize = Math.Clamp(pageSize, 1, 1000);

            query = query.Skip((page - 1) * pageSize).Take(pageSize);

            return await query.ToListAsync(cancellationToken);
        }

        private static PropertyInfo? ResolveTemporalProperty(Dictionary<string, PropertyInfo> entityProps)
        {
            foreach (var candidate in TemporalPropertyCandidates)
            {
                if (entityProps.TryGetValue(candidate, out var p) &&
                    (p.PropertyType == typeof(DateTime) || p.PropertyType == typeof(DateTime?)))
                {
                    // Return the actual property (preserves correct name/casing and type)
                    return p;
                }
            }
            return null;
        }

        private static IQueryable<T> ApplyOrdering(IQueryable<T> source, ParameterExpression param, PropertyInfo prop, bool descending)
        {
            var propAccess = Expression.Property(param, prop);
            var delegateType = typeof(Func<,>).MakeGenericType(typeof(T), prop.PropertyType);
            var lambda = Expression.Lambda(delegateType, propAccess, param);

            string methodName = descending ? "OrderByDescending" : "OrderBy";

            var method = typeof(Queryable).GetMethods(BindingFlags.Public | BindingFlags.Static)
                .Where(m => m.Name == methodName && m.GetParameters().Length == 2)
                .Select(m => new { Method = m, Args = m.GetGenericArguments() })
                .First(m => m.Args.Length == 2)
                .Method
                .MakeGenericMethod(typeof(T), prop.PropertyType);

            return (IQueryable<T>)method.Invoke(null, new object[] { source, lambda })!;
        }

        private static IQueryable<T> ApplyDateFilter(IQueryable<T> source, PropertyInfo prop, DateTime boundary, bool isBefore)
        {
            var param = Expression.Parameter(typeof(T), "e");
            var left = Expression.Property(param, prop);

            // Right side constant typed to property type (handles DateTime and DateTime?)
            Expression right = prop.PropertyType == typeof(DateTime)
                ? Expression.Constant(boundary, typeof(DateTime))
                : Expression.Convert(Expression.Constant(boundary, typeof(DateTime)), typeof(DateTime?));

            Expression body = isBefore
                ? Expression.LessThanOrEqual(left, right)
                : Expression.GreaterThanOrEqual(left, right);

            var lambda = Expression.Lambda<Func<T, bool>>(body, param);
            return source.Where(lambda);
        }

        private static Expression? BuildTypedConstant(object value, Type targetType)
        {
            try
            {
                // Nullable<T>
                if (targetType.IsGenericType && targetType.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    var underlying = Nullable.GetUnderlyingType(targetType)!;
                    var converted = ConvertToType(value, underlying);
                    if (converted == null) return null;

                    var nonNullableConst = Expression.Constant(converted, underlying);
                    return Expression.Convert(nonNullableConst, targetType);
                }

                // Non-nullable
                var nonNull = ConvertToType(value, targetType);
                if (nonNull == null) return null;

                return Expression.Constant(nonNull, targetType);
            }
            catch
            {
                return null;
            }
        }

        private static object? ConvertToType(object value, Type targetType)
        {
            try
            {
                if (targetType.IsEnum)
                {
                    if (value is string s)
                        return Enum.Parse(targetType, s, ignoreCase: true);
                    return Enum.ToObject(targetType, Convert.ChangeType(value, Enum.GetUnderlyingType(targetType), CultureInfo.InvariantCulture)!);
                }

                if (targetType == typeof(Guid))
                {
                    if (value is Guid g) return g;
                    if (value is string sg) return Guid.Parse(sg);
                }

                if (targetType == typeof(string))
                    return Convert.ToString(value, CultureInfo.InvariantCulture);

                return Convert.ChangeType(value, targetType, CultureInfo.InvariantCulture);
            }
            catch
            {
                return null;
            }
        }
    }
}