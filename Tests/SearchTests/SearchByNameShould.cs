using DrinkManagerWeb.Services;
using FluentAssertions;
using System.Linq;
using Xunit;

namespace Tests.SearchTests
{
    public class SearchByNameShould : IClassFixture<TestWithSqlite>
    {
        private readonly DrinkSearchService _sut;

        public SearchByNameShould(TestWithSqlite fixture)
        {
            _sut = new DrinkSearchService(fixture.Context);
        }

        [Fact]
        public void Get_all_drinks_from_the_database_if_query_text_was_an_empty_string()
        {
            var drinks = _sut.SearchByName("");
            drinks.Should().HaveCount(42);
        }

        [Fact]
        public void Get_correct_amount_of_drinks_for_search_query_cuba()
        {
            var drinks = _sut.SearchByName("cuba");
            drinks.Should().HaveCount(2);
        }

        [Theory]
        [InlineData("mojito")]
        [InlineData("MOjIto")]
        public void Be_CaseInsensitive(string query)
        {
            var drink = _sut.SearchByName(query);
            drink.Should().HaveCount(1);
            drink.First().Name.Should().Be("Mojito #3");
        }
    }
}
