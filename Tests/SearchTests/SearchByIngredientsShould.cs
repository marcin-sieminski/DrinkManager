using BLL.Enums;
using DrinkManagerWeb.Services;
using FluentAssertions;
using System.Collections.Generic;
using Xunit;

namespace Tests.SearchTests
{
    public class SearchByIngredientsShould : IClassFixture<TestWithSqlite>
    {
        private readonly DrinkSearchService _sut;

        public SearchByIngredientsShould(TestWithSqlite fixture)
        {
            _sut = new DrinkSearchService(fixture.Context);
        }

        [Fact]
        public void Get_correct_number_of_drinks_with_one_specific_ingredient()
        {
            var drinks = _sut.SearchByIngredients(new SortedSet<string> { "rum" }, SearchDrinkOption.Any);
            drinks.Should().HaveCount(10);
        }

        [Fact]
        public void Get_correct_number_of_drinks_with_two_specific_ingredients()
        {
            var drinks = _sut.SearchByIngredients(new SortedSet<string> { "lime", "rum" }, SearchDrinkOption.All);
            drinks.Should().HaveCount(5);
        }

        [Fact]
        public void Get_all_drinks_if_search_query_is_an_empty_string()
        {
            var drinks = _sut.SearchByIngredients(new SortedSet<string> { "" }, SearchDrinkOption.All);
            drinks.Should().HaveCount(42);
        }

        [Theory]
        [InlineData("rum")]
        [InlineData("rUM")]
        public void Be_CaseInsensitive(string query)
        {
            var drinks = _sut.SearchByIngredients(new SortedSet<string> { query }, SearchDrinkOption.All);
            drinks.Should().HaveCount(10);
        }
    }
}
