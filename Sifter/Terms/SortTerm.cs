using System.Linq;
using System.Linq.Expressions;
using System.Reflection;


namespace Sifter.Terms {

    internal class SortTerm {

        public string Identifier { get; }

        private readonly bool isDescending;



        public SortTerm(string str) {
            if (str.StartsWith('-')) {
                Identifier = str.Substring(1);
                isDescending = true;
            }
            else {
                Identifier = str;
                isDescending = false;
            }
        }



        public IQueryable<TEntity> ApplySort<TEntity>(
            IQueryable<TEntity> query,
            PropertyInfo propertyInfo,
            bool isSecondarySort
        ) {
            //TODO maybe expression switch this
            var command = isDescending ? isSecondarySort ? "ThenByDescending" : "OrderByDescending" :
                isSecondarySort ? "ThenBy" : "OrderBy";
            var type = typeof(TEntity);
            var parameterExpr = Expression.Parameter(type);

            var propertyExpr = createPropertyExpression(parameterExpr, Identifier);
            var memberAccess = Expression.MakeMemberAccess(propertyExpr, propertyInfo);
            var memberAccessExpr = Expression.Lambda(memberAccess, parameterExpr);
            var orderByExpr = Expression.Call(
                typeof(Queryable),
                command,
                new[] { type, propertyInfo.PropertyType },
                query.Expression,
                Expression.Quote(memberAccessExpr)
            );
            return query.Provider.CreateQuery<TEntity>(orderByExpr);
        }



        private static Expression createPropertyExpression(Expression parameterExpr, string identifier) {
            if (!identifier.Contains('.')) {
                return parameterExpr;
            }

            var splits = identifier.Split('.').SkipLast(1);
            return splits.Aggregate(parameterExpr, Expression.Property);
        }

    }

}