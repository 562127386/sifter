using System;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Sifter;
using Tests.Helpers;
using Xunit;
using Xunit.Abstractions;


namespace Tests {

    public class FilterTests {

        private readonly ITestOutputHelper testOutputHelper;



        //TODO The tests in this class assume that the name to property mapping is not case sensitive
        public FilterTests(ITestOutputHelper testOutputHelper) {
            this.testOutputHelper = testOutputHelper;
        }

        //TODO create some dynamic tests, combining different property types with variable types and operators


        [Theory]
        [InlineData("balance>0", "([x].[Balance] > 0)")]
        [InlineData("balance>-0", "([x].[Balance] > 0)")]
        [InlineData("balance>0.1", "([x].[Balance] > 0.1)")]
        [InlineData("balance>-0.1", "([x].[Balance] > -0.1)")]
        [InlineData("balance>2.1", "([x].[Balance] > 2.1)")]
        [InlineData("balance > 2.1", "([x].[Balance] > 2.1)")]
//        [InlineData("balance> - 12", "([x].[Balance] > -12)")] TODO the json parser doesnt like whitespace between the minus and the number
        [InlineData("balance> -12", "([x].[Balance] > -12)")]
        [InlineData("balance ==0", "([x].[Balance] = 0)")]
        [InlineData("balance !=0", "([x].[Balance] <> 0)")]
        [InlineData("balance>=0", "([x].[Balance] >= 0)")]
        [InlineData("balance<0", "([x].[Balance] < 0)")]
        [InlineData("balance<=0", "([x].[Balance] <= 0)")]
        [InlineData("steamid !=\"lmao\"", "(([x].[SteamId] <> N'lmao') OR [x].[SteamId] IS NULL)")]
        [InlineData("steamid ==\"lmao\"", "([x].[SteamId] = N'lmao')")]
        [InlineData("steamid !=*\"lmao\"", "NOT ([x].[SteamId] LIKE N'lmao')")]
        [InlineData("steamid ==*\"lmao\"", "[x].[SteamId] LIKE N'lmao'")]
        public void TestFiltering(string filterInput, string expectedOutput) {
            var sql = testFiltering(filterInput);

            testOutputHelper.WriteLine(sql);
            Assert.EndsWith(expectedOutput, Regex.Replace(sql, "0+1E0", ""));
        }
        
        [Theory]
        [InlineData("-balance>-12")]
        [InlineData("balance>-")]
        [InlineData("balance>>-12")]
        [InlineData("balance><-12")]
        [InlineData("balance>'lmao'")]
        [InlineData("balance>\"lmao\"")]
        [InlineData("balance>lmao")]
        [InlineData("balance>true")]
        [InlineData("steamid>\"asdas\"")]
        [InlineData("steamid !=0")]
        [InlineData("steamid !=lmao")]
        [InlineData("steamid ==`lmao`")]
        [InlineData("steamid =='lmao'")]
        [InlineData("balance>+0")]
        [InlineData("balance>+0.1")]
        public void TestInvalidFiltering(string filterInput) {
            var sql = testFiltering(filterInput);

            testOutputHelper.WriteLine(sql);
            Assert.DoesNotContain("WHERE", sql.Replace("WHERE [x].[Discriminator]", ""));
        }



        private static string testFiltering(string filterInput) {
            var sifter = new MySifterService();
            var context = DataContext.Instance;
            var model = new SifterModel {
                Filter = filterInput
            };

            var query = context.SubUsers.Include(x => x.Inner);
            var sql = sifter.Sift(query, model).ToSql();
            return Regex.Replace(sql, "0+1E0", "");//Removes all the trailing zeroes for easier testing and legibility
        }

    }

}