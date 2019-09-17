using System.Linq;
using System.Linq.Expressions;
using System.Reflection;


namespace Sifter {

    internal static class QueryableExtensions {

        public static IQueryable<TEntity> ApplySort<TEntity>(
            this IQueryable<TEntity> query,
            string identifier,
            PropertyInfo propertyInfo,
            SortTerm sortTerm,
            bool isNestedSort
        ) {
            var command = sortTerm.IsDescending ? isNestedSort ? "ThenByDescending" : "OrderByDescending" :
                isNestedSort ? "ThenBy" : "OrderBy";
            var type = typeof(TEntity);
            var parameter = Expression.Parameter(type);

            Expression propertyValue = parameter;

            if (identifier.IsNested()) {
                var splits = identifier.Split('.').SkipLast(1);

                foreach (var split in splits) {
                    propertyValue = Expression.PropertyOrField(propertyValue, split);
                }
            }

            var memberAccess = Expression.MakeMemberAccess(propertyValue, propertyInfo);
            var memberAccessExpression = Expression.Lambda(memberAccess, parameter);
            var orderByExpression = Expression.Call(typeof(Queryable), command,
                new[] { type, propertyInfo.PropertyType },
                query.Expression, Expression.Quote(memberAccessExpression));
            return query.Provider.CreateQuery<TEntity>(orderByExpression);
        }

    }

}