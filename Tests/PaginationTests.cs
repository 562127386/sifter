using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Sifter.Models;
using Tests.Helpers;
using Xunit;
using Xunit.Abstractions;


namespace Tests {

    [SuppressMessage("ReSharper", "StringLiteralTypo")]
    public class PaginationTests {

        private readonly ITestOutputHelper testOutputHelper;



        //TODO The tests in this class assume that the name to property mapping is not case sensitive, that the default page size is 25, and that the max page is 100
        //TODO this does not yet test min/max page size
        public PaginationTests(ITestOutputHelper testOutputHelper) {
            this.testOutputHelper = testOutputHelper;
        }



        [Theory]
        [InlineData("-1", "1", "SELECT TOP(1)")]
        [InlineData("-1", "-1", "SELECT TOP(1)")]
        [InlineData("1", "200", "SELECT TOP(100)")]
        [InlineData("-1", "200", "SELECT TOP(100)")]
        [InlineData("2.3", "200", "SELECT TOP(100)")]
        [InlineData("-", "0", "SELECT TOP(1)")]
        [InlineData("1", "1", "SELECT TOP(1)")]
        [InlineData("1", "2", "SELECT TOP(2)")]
        [InlineData("0", "0", "SELECT TOP(1)")]
        [InlineData("", "", "SELECT TOP(25)")]
        [InlineData("1", "", "SELECT TOP(25)")]
        [InlineData("1", "23", "SELECT TOP(23)")]
        [InlineData("", "23", "SELECT TOP(23)")]
        [InlineData("2", "2", "OFFSET 2 ROWS FETCH NEXT 2 ROWS ONLY")]
        [InlineData("2", "23", "OFFSET 23 ROWS FETCH NEXT 23 ROWS ONLY")]
        [InlineData("3", "23", "OFFSET 46 ROWS FETCH NEXT 23 ROWS ONLY")]
        [InlineData("3", "", "OFFSET 50 ROWS FETCH NEXT 25 ROWS ONLY")]
        public void TestPagination(string page, string pageSize, string expectedOutput) {
            var sql = testPagination(page, pageSize);

            testOutputHelper.WriteLine(sql);

            if (expectedOutput.Contains("TOP")) {
                Assert.StartsWith(expectedOutput, sql);
            }
            else {
                Assert.EndsWith(expectedOutput, sql);
            }
        }



        //There are no tests for invalid pagination requests, because if a request is invalid, it will always use the default pagination options



        private static string testPagination(string page, string pageSize) {
            var sifter = new MySifterService();
            var context = DataContext.Instance;
            var model = new SifterModel {
                Page = page,
                PageSize = pageSize
            };

            var query = context.SubUsers.Include(x => x.Inner);
            var sql = sifter.Sift(query, model).ToSql();
            return sql;
        }

    }

}