using System.Linq;
using Microsoft.EntityFrameworkCore;
using Sifter;
using Tests.Helpers;
using Xunit;
using Xunit.Abstractions;


namespace Tests {

    public class FilterTests {

        private readonly ITestOutputHelper testOutputHelper;



        //The tests in this class assume that the name to property mapping is not case sensitive
        public FilterTests(ITestOutputHelper testOutputHelper) {
            this.testOutputHelper = testOutputHelper;
        }



        [Theory]
        [InlineData("balance>0", "([x].[Balance] > 0)")]
        [InlineData("balance>0.1", "([x].[Balance] > 0.1)")]
        [InlineData("balance>2.1", "([x].[Balance] > 0.1)")]
        [InlineData("balance>-", "([x].[Balance] > 0.1)")]
        [InlineData("-balance>-12", "([x].[Balance] > 0.1)")]
        [InlineData("balance>-12", "([x].[Balance] > -12)")]
        [InlineData("balance>>-12", "([x].[Balance] > -12)")]
        [InlineData("balance><-12", "([x].[Balance] > -12)")]
        [InlineData("balance>true", "([x].[Balance] > -12)")]
        [InlineData("balance>lmao", "([x].[Balance] > -12)")]
        public void TestFiltering(string filterInput, string expectedOutput) {
            var sql = testFiltering(filterInput);

            testOutputHelper.WriteLine(sql);
            Assert.EndsWith(expectedOutput, sql);
        }



        private static string testFiltering(string filterInput) {
            var sifter = new MySifterService();
            var context = DataContext.Instance;
            var model = new SifterModel {
                Filter = filterInput
            };

            var query = context.SubUsers.Include(x => x.Inner);
            var sql = sifter.Sift(query, model).ToSql();
            return sql;
        }

    }

}