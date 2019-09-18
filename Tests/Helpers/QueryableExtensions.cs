using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.EntityFrameworkCore.Storage;


namespace Tests.Helpers {

    public static class QueryableExtensions {

        private static readonly TypeInfo queryCompilerTypeInfo = typeof(QueryCompiler).GetTypeInfo();

        private static readonly FieldInfo queryCompilerField = typeof(EntityQueryProvider).GetTypeInfo().DeclaredFields
            .First(x => x.Name == "_queryCompiler");

        private static readonly FieldInfo queryModelGeneratorField = typeof(QueryCompiler).GetTypeInfo().DeclaredFields
            .First(x => x.Name == "_queryModelGenerator");

        private static readonly FieldInfo dataBaseField =
            queryCompilerTypeInfo.DeclaredFields.Single(x => x.Name == "_database");

        private static readonly PropertyInfo databaseDependenciesField =
            typeof(Database).GetTypeInfo().DeclaredProperties.Single(x => x.Name == "Dependencies");



        public static string ToSql<TEntity>(this IQueryable<TEntity> query) {
            var queryCompiler = (QueryCompiler) queryCompilerField.GetValue(query.Provider);
            var queryModelGenerator = (QueryModelGenerator) queryModelGeneratorField.GetValue(queryCompiler);
            var queryModel = queryModelGenerator.ParseQuery(query.Expression);
            var database = dataBaseField.GetValue(queryCompiler);
            var databaseDependencies = (DatabaseDependencies) databaseDependenciesField.GetValue(database);
            var queryCompilationContext = databaseDependencies.QueryCompilationContextFactory.Create(false);
            var modelVisitor = (RelationalQueryModelVisitor) queryCompilationContext.CreateQueryModelVisitor();
            modelVisitor.CreateQueryExecutor<TEntity>(queryModel);
            var sql = modelVisitor.Queries.First().ToString();

            return sql;
        }

    }

}