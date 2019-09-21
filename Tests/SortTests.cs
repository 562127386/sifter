using Microsoft.EntityFrameworkCore;
using Sifter;
using Tests.Helpers;
using Xunit;
using Xunit.Abstractions;


namespace Tests {

    public class SortTests {

        private readonly ITestOutputHelper testOutputHelper;



        //TODO The tests in this class assume that the name to property mapping is not case sensitive
        public SortTests(ITestOutputHelper testOutputHelper) {
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
        [InlineData("Id,Id", "ORDER BY [x].[Id]")]
        [InlineData("-Id,Id", "ORDER BY [x].[Id] DESC")]
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
        [InlineData("-Id,,,,,,")]
        [InlineData(",,,',`,\\,,-Id,,IdId,-Id,Id,df3q213fs__ef_sdé\\114wq4t!$!#@!#42,Id,,,m")]
        public void TestInvalidSorting(string sortInput) {
            var sql = testSorting(sortInput);

            testOutputHelper.WriteLine(sql);
            Assert.DoesNotContain("ORDER", sql);
        }



        private static string testSorting(string sortInput) {
            var sifter = new MySifterService();
            var context = DataContext.Instance;
            var model = new SifterModel {
                Sort = sortInput
            };

            var query = context.SubUsers.Include(x => x.Inner);
            var sql = sifter.Sift(query, model).ToSql(); //TODO a select() must always be after the sift
            return sql;
        }

    }

}