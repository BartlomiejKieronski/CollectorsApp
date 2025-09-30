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
        /// <summary>
        /// Represents a set of reserved property names in the DTO that are used for special query operations. 
        /// </summary>
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

        /// <summary>
        /// Represents a collection of property names commonly used to identify temporal or date-related fields.
        /// </summary>
        /// <remarks>This array includes common naming conventions for properties that represent
        /// timestamps or dates,  such as "TimeStamp", "InsertDate", and "AccountCreationDate". It can be used to
        /// identify or filter  temporal properties in a dataset or object model.</remarks>
        private static readonly string[] TemporalPropertyCandidates =
        {
            "TimeStamp",
            "Timestamp",
            "InsertDate",
            "AccountCreationDate",
            "DateOfIssue"
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryRepository"/> class with the specified database context.
        /// </summary>
        /// <param name="context">The database context used to interact with the application's data store.  This parameter cannot be <see
        /// langword="null"/>.</param>
        public QueryRepository(appDatabaseContext context) : base(context) { }

        /// <summary>
        /// Queries entities based on the specified data transfer object (DTO).
        /// </summary>
        /// <remarks>This method uses a default <see cref="CancellationToken"/> of <see
        /// cref="CancellationToken.None"/>.  To specify a cancellation token, use the overload that accepts
        /// one.</remarks>
        /// <param name="entity">The data transfer object (DTO) containing the criteria for querying entities.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an enumerable collection of
        /// entities that match the specified criteria.</returns>
        public virtual async Task<IEnumerable<T>> QueryEntity(TDTO entity)
        {
            return await QueryEntity(entity, CancellationToken.None);
        }

        /// <summary>
        /// Queries the database for entities of type <typeparamref name="T"/> based on the specified filter criteria.
        /// </summary>
        /// <remarks>The method supports filtering based on the properties of the <typeparamref
        /// name="TDTO"/> object.  - String properties are filtered using case-insensitive substring matching. -
        /// Temporal properties can be filtered using "Before" and "After" criteria. - Additional filtering options
        /// include multi-word keyword searches and phrase searches.  The results can be ordered by specifying the
        /// "OrderBy" property in the <typeparamref name="TDTO"/> object,  with an optional "SortDescending" flag. If no
        /// ordering is specified, the results are ordered by the "Id" property  or the first available property. 
        /// Paging is supported via the "Page" and "NumberOfRecords" properties in the <typeparamref name="TDTO"/>
        /// object.  The default page size is 10, and the maximum page size is 1000.</remarks>
        /// <param name="entity">An instance of <typeparamref name="TDTO"/> containing the filter criteria. If <paramref name="entity"/> is
        /// null,  all entities of type <typeparamref name="T"/> are returned.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to cancel the asynchronous operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a list of entities of type 
        /// <typeparamref name="T"/> that match the specified filter criteria. If no entities match, an empty list is
        /// returned.</returns>
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

        /// <summary>
        /// Resolves the first property from the provided dictionary that matches a temporal property name and has a
        /// type of <see cref="DateTime"/> or <see cref="DateTime?"/>.
        /// </summary>
        /// <remarks>Temporal property names are determined by the predefined list of candidates in
        /// <c>TemporalPropertyCandidates</c>. The method performs a case-sensitive search and ensures the property type
        /// is either <see cref="DateTime"/> or nullable <see cref="DateTime?"/>.</remarks>
        /// <param name="entityProps">A dictionary containing property names as keys and their corresponding <see cref="PropertyInfo"/> objects as
        /// values.</param>
        /// <returns>The <see cref="PropertyInfo"/> of the first matching temporal property, or <see langword="null"/> if no
        /// match is found.</returns>
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

        /// <summary>
        /// Applies ordering to the specified <see cref="IQueryable{T}"/> source based on the given property and sort
        /// direction.
        /// </summary>
        /// <remarks>This method dynamically constructs and applies an ordering expression to the
        /// queryable source.  It supports both ascending and descending order based on the <paramref
        /// name="descending"/> parameter.</remarks>
        /// <param name="source">The source queryable to which the ordering will be applied.</param>
        /// <param name="param">The parameter expression representing the entity type in the query.</param>
        /// <param name="prop">The property on which to base the ordering.</param>
        /// <param name="descending">A value indicating whether the ordering should be descending.  <see langword="true"/> to apply descending
        /// order; otherwise, <see langword="false"/> for ascending order.</param>
        /// <returns>A new <see cref="IQueryable{T}"/> with the specified ordering applied.</returns>
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

        /// <summary>
        /// Filters the elements of the specified queryable source based on a date comparison.
        /// </summary>
        /// <param name="source">The queryable source to filter. Cannot be <see langword="null"/>.</param>
        /// <param name="prop">The property to compare against the specified date boundary. Must be of type <see cref="DateTime"/> or <see
        /// cref="Nullable{DateTime}"/>.</param>
        /// <param name="boundary">The date boundary to use for filtering.</param>
        /// <param name="isBefore">A value indicating whether to filter for elements where the property value is before or on the boundary
        /// date. If <see langword="true"/>, filters for elements where the property value is less than or equal to the
        /// boundary. If <see langword="false"/>, filters for elements where the property value is greater than or equal
        /// to the boundary.</param>
        /// <returns>A queryable containing the elements from the source that satisfy the date filter condition.</returns>
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

        /// <summary>
        /// Builds a typed constant expression for the specified value and target type.
        /// </summary>
        /// <remarks>This method attempts to convert the provided <paramref name="value"/> to the
        /// specified <paramref name="targetType"/>. If the conversion fails or is not possible, the method returns <see
        /// langword="null"/>. For nullable types, the method handles the conversion to the underlying type and wraps
        /// the result in a nullable expression.</remarks>
        /// <param name="value">The value to be converted into a typed constant expression.</param>
        /// <param name="targetType">The target type of the constant expression. This can be a nullable or non-nullable type.</param>
        /// <returns>An <see cref="Expression"/> representing the typed constant if the conversion is successful; otherwise, <see
        /// langword="null"/>.</returns>
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

        /// <summary>
        /// Converts the specified value to the specified target type.
        /// </summary>
        /// <remarks>This method supports conversion to enums, <see cref="Guid"/>, and other types
        /// supported by <see cref="Convert.ChangeType"/>. For enums, the method can parse a string representation of
        /// the enum value or convert a numeric value to the corresponding enum. For <see cref="Guid"/>, the method can
        /// parse a string representation or return the value directly if it is already a <see cref="Guid"/>. If the
        /// target type is <see cref="string"/>, the value is converted to its string representation. If the conversion
        /// is not possible, the method returns <see langword="null"/> instead of throwing an exception.</remarks>
        /// <param name="value">The value to be converted. Must be compatible with the <paramref name="targetType"/>.</param>
        /// <param name="targetType">The type to which the <paramref name="value"/> should be converted.</param>
        /// <returns>An object representing the converted value, or <see langword="null"/> if the conversion fails.</returns>
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