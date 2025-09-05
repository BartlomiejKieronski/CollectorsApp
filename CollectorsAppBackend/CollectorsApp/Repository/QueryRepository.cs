using CollectorsApp.Data;
using CollectorsApp.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Reflection;

namespace CollectorsApp.Repository
{
    /// <summary>
    /// Generic reflective query repository. Builds a predicate from non-null DTO properties
    /// plus supports Before / After (date range), ordering and paging.
    /// </summary>
    public class QuerryRepository<T, TDTO> : CRUDImplementation<T>, IQueryInterface<T, TDTO>
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
            "TimeStamp"
        };

        // Ordered preference of temporal property names we’ll auto-detect.
        private static readonly string[] TemporalPropertyCandidates =
        {
            "TimeStamp",        
            "Timestamp",
            "InsertDate",       
            "AccountCreationDate",
            "DateOfIssue"
        };

        public QuerryRepository(appDatabaseContext context) : base(context) { }

        // Interface method (kept signature). CancellationToken-aware overload provided.
        public virtual async Task<IEnumerable<T>> QueryEntity(TDTO entity) { 
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

            foreach (var dtoProp in dtoProps)
            {
                if (ReservedDtoMembers.Contains(dtoProp.Name))
                    continue;

                var value = dtoProp.GetValue(entity);
                if (value == null) continue;

                if (!entityProps.TryGetValue(dtoProp.Name, out var entProp))
                    continue;

                // Skip empty string filters
                if (entProp.PropertyType == typeof(string) && value is string s && string.IsNullOrWhiteSpace(s))
                    continue;

                var left = Expression.Property(param, entProp);

                // Ensure right constant matches (or can be converted to) property type
                Expression right = Expression.Constant(value, entProp.PropertyType.IsAssignableFrom(value.GetType())
                    ? entProp.PropertyType
                    : value.GetType());

                Expression condition;
                if (entProp.PropertyType == typeof(string))
                {
                    // e.Property.Contains(value)
                    var containsMethod = typeof(string).GetMethod(nameof(string.Contains), new[] { typeof(string) });
                    condition = Expression.Call(left, containsMethod!, Expression.Constant(value, typeof(string)));
                }
                else
                {
                    if (entProp.PropertyType.IsGenericType &&
                        entProp.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                    {
                        // Convert constant to nullable wrapper if needed
                        if (right.Type != entProp.PropertyType)
                            right = Expression.Convert(right, entProp.PropertyType);
                    }
                    else if (right.Type != entProp.PropertyType)
                    {
                        // Attempt conversion for numeric/enums where DTO property type differs
                        try
                        {
                            right = Expression.Convert(right, entProp.PropertyType);
                        }
                        catch
                        {
                            // Skip if incompatible
                            continue;
                        }
                    }

                    condition = Expression.Equal(left, right);
                }

                predicate = predicate == null ? condition : Expression.AndAlso(predicate, condition);
            }
            if (predicate != null)
            {
                var lambda = Expression.Lambda<Func<T, bool>>(predicate, param);
                query = query.Where(lambda);
            }

            // Date range filters
            var beforeProp = typeof(TDTO).GetProperty("Before", BindingFlags.Public | BindingFlags.Instance);
            var afterProp = typeof(TDTO).GetProperty("After", BindingFlags.Public | BindingFlags.Instance);

            var temporalPropertyName = ResolveTemporalPropertyName(entityProps);

            if (temporalPropertyName != null)
            {
                if (beforeProp?.GetValue(entity) is DateTime beforeValue)
                    query = query.Where(e => EF.Property<DateTime?>(e, temporalPropertyName) <= beforeValue);

                if (afterProp?.GetValue(entity) is DateTime afterValue)
                    query = query.Where(e => EF.Property<DateTime?>(e, temporalPropertyName) >= afterValue);
            }

            // Ordering
            string orderBy = dtoProps.FirstOrDefault(p => p.Name.Equals("OrderBy", StringComparison.OrdinalIgnoreCase))
                                ?.GetValue(entity)?.ToString() ?? "Id";

            bool sortDescending = dtoProps.FirstOrDefault(p => p.Name.Equals("SortDescending", StringComparison.OrdinalIgnoreCase))
                                ?.GetValue(entity) as bool? ?? false;

            if (entityProps.TryGetValue(orderBy, out var orderProp))
            {
                query = ApplyOrdering(query, param, orderProp, sortDescending);
            }
            else
            {
                // Fallback deterministic ordering if Id is not present
                if (!entityProps.ContainsKey("Id"))
                {
                    var firstProp = entityProps.Values.FirstOrDefault();
                    if (firstProp != null)
                    {
                        query = ApplyOrdering(query, param, firstProp, false);
                    }
                }
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

        private static string? ResolveTemporalPropertyName(Dictionary<string, PropertyInfo> entityProps)
        {
            foreach (var candidate in TemporalPropertyCandidates)
            {
                if (entityProps.ContainsKey(candidate))
                    return candidate;
            }
            return null;
        }

        private static IQueryable<T> ApplyOrdering(IQueryable<T> source, ParameterExpression param, PropertyInfo prop, bool descending)
        {
            var propAccess = Expression.Property(param, prop);
            var lambda = Expression.Lambda(propAccess, param);

            string methodName = descending ? "OrderByDescending" : "OrderBy";

            var method = typeof(Queryable).GetMethods(BindingFlags.Public | BindingFlags.Static)
                .Where(m => m.Name == methodName && m.GetParameters().Length == 2)
                .Select(m => new { Method = m, Args = m.GetGenericArguments() })
                .First(m => m.Args.Length == 2)
                .Method
                .MakeGenericMethod(typeof(T), prop.PropertyType);

            return (IQueryable<T>)method.Invoke(null, new object[] { source, lambda })!;
        }
    }
}
