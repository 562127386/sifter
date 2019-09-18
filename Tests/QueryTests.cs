using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using Sifter;
using Xunit;
using Xunit.Abstractions;


namespace Tests {

    public class QueryTests {

        private readonly ITestOutputHelper testOutputHelper;



        //The tests in this class assume that the name to property mapping is not case sensitive
        public QueryTests(ITestOutputHelper testOutputHelper) {
            this.testOutputHelper = testOutputHelper;
        }


        [Theory]
        [InlineData("Balance", "ORDER BY [x].[Balance]")]
        [InlineData("-Balance", "ORDER BY [x].[Balance] DESC")]
        [InlineData("Id,Balance", "ORDER BY [x].[Id], [x].[Balance]")]
        [InlineData("Balance,Id", "ORDER BY [x].[Balance], [x].[Id]")]
        [InlineData("-Id,Balance", "ORDER BY [x].[Id] DESC, [x].[Balance]")]
        [InlineData("Id,-Balance", "ORDER BY [x].[Id], [x].[Balance] DESC")]
        [InlineData("-Balance,Id", "ORDER BY [x].[Balance] DESC, [x].[Id]")]
        [InlineData("Balance,-Id", "ORDER BY [x].[Balance], [x].[Id] DESC")]
        [InlineData("-Balance,-Id", "ORDER BY [x].[Balance] DESC, [x].[Id] DESC")]
        [InlineData("-Id,-Balance", "ORDER BY [x].[Id] DESC, [x].[Balance] DESC")]
        [InlineData("-notExisting,notSortable,-Id", "ORDER BY [x].[Id] DESC")]
        [InlineData("-Id,,,,,,", "ORDER BY [x].[Id] DESC")]
        [InlineData("Id,Id", "ORDER BY [x].[Id]")]
        [InlineData("-Id,Id", "ORDER BY [x].[Id] DESC")]
        [InlineData(",,,',`,\\,,-Id,,IdId,-Id,Id,df3q213fs__ef_sdé\\114wq4t!$!#@!#42,Id,,,m", "ORDER BY [x].[Id] DESC")]
        [InlineData("SubUserProp", "ORDER BY [x].[SubUserProp]")]
        [InlineData("Inner.Number", "ORDER BY [x.Inner].[Number]")]
        [InlineData("Inner,Inner.Number", "ORDER BY [x.Inner].[Number]")]
        [InlineData("Inner.Brumber,Inner.Number", "ORDER BY [x.Inner].[Number]")]
        [InlineData("bAlAnce", "ORDER BY [x].[Balance]")]
        [InlineData("ID", "ORDER BY [x].[Id]")]
        [InlineData("id", "ORDER BY [x].[Id]")]
        [InlineData("iD", "ORDER BY [x].[Id]")]
        public void TestSorting(string sortInput, string expectedOutput) {
            var sql = testSorting(sortInput);

            testOutputHelper.WriteLine(sql);
            Assert.EndsWith(expectedOutput, sql);
        }



        [Theory]
        [InlineData("-NotSortable")]
        [InlineData("NotSortable")]
        [InlineData("NotExisting")]
        [InlineData("-NotExisting")]
        [InlineData("-NotExisting,NotSortable")]
        [InlineData(",")]
        [InlineData(",-,,--")]
        [InlineData("--id")]
        [InlineData("Inner")]
        [InlineData("Inner.Brumber,")]
        [InlineData("Inner.Brumber")]
        [InlineData("Inner.Inner.Number")]
        [InlineData("id.id")]
        public void TestInvalidSorting(string sortInput) {
            var sql = testSorting(sortInput);

            testOutputHelper.WriteLine(sql);
            Assert.DoesNotContain("ORDER", sql);
        }



        private static string testSorting(string sortInput) {
            var sifter = new MySifterServiceService();
            var context = new DataContext(new DbContextOptionsBuilder()
                .UseSqlServer(
                    @"Server=(localdb)\mssqllocaldb;Database=Sifter.Tests;Trusted_Connection=True;ConnectRetryCount=0")
                .Options);
            var model = new SifterModel {
                Sort = sortInput
            };

            var query = context.SubUsers.Include(x => x.Inner);
            var sql = sifter.Sift(query, model).ToSql();
            return sql;
        }

    }


    public class MySifterServiceService : SifterServiceService {

        protected override void onSifterBuild(SifterBuilder builder) {
            builder.IndexDbSets<DataContext>();

            builder.Properties<User>()
                .CanFilterAndSort(u => u.Id)
                .CanFilterAndSort(u => u.Balance);

            builder.Properties<SubUser>()
                .CanFilterAndSort(s => s.SubUserProp);

            builder.Properties<Inner>()
                .CanFilterAndSort(c => c.Id)
                .CanFilterAndSort(c => c.Number);
        }

    }


    public class DataContext : DbContext {

        public DataContext(DbContextOptions options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<SubUser> SubUsers { get; set; }
        public DbSet<Inner> Children { get; set; }

    }


    public class User {

        public int Id { get; set; }
        public string SteamId { get; set; }
        public int Balance { get; set; }
        public Inner Inner { get; set; }
        public int InnerId { get; set; }
        public int NotSortable { get; set; }

    }


    public class SubUser : User {

        public int SubUserProp { get; set; }

    }


    public class Inner {

        public int Id { get; set; }
        public int Number { get; set; }

    }


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