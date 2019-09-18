using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using JetBrains.Annotations;
using Sifter.Extensions;


namespace Sifter.Terms {

    internal class SortTerm {

        [CanBeNull]
        public string Identifier { get; }

        public bool IsDescending { get; }



        public SortTerm(string str) {
            if (str.StartsWith('-')) {
                Identifier = str.Substring(1);
                IsDescending = true;
            }
            else {
                Identifier = str;
                IsDescending = false;
            }
        }



        public IQueryable<TEntity> ApplySort<TEntity>(
            IQueryable<TEntity> query,
            string identifier,
            PropertyInfo propertyInfo,
            bool isNestedSort
        ) {
            //TODO maybe expression switch this
            var command = IsDescending ? isNestedSort ? "ThenByDescending" : "OrderByDescending" :
                isNestedSort ? "ThenBy" : "OrderBy";
            var type = typeof(TEntity);
            var parameter = Expression.Parameter(type);

            Expression propertyValue = parameter;

            if (identifier.IsNested()) {
                var splits = identifier.Split('.').SkipLast(1);

                propertyValue = splits.Aggregate(propertyValue, Expression.PropertyOrField);
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