using DrinkManagerWeb.Services;
using FluentAssertions;
using System.Linq;
using Xunit;

namespace Tests
{
    public class DrinkFavouriteServiceShould : IClassFixture<TestWithSqlite>
    {
        private readonly TestWithSqlite _fixture;

        public DrinkFavouriteServiceShould(TestWithSqlite fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public void ChangeIsFavouriteToTrueIfMakeItFavouriteWasCalled()
        {
            var list = _fixture.Repository.GetAllDrinks();
            var favouriteService = new DrinkFavouriteService();
            var drink = list.First(x => x.Name.Contains("Cuba Libre"));

            favouriteService.MakeItFavourite(drink);

            drink.IsFavourite.Should().Be(true);
        }
        [Fact]
        public void ChangeIsFavouriteToFalseIfMakeItNotFavouriteWasCalled()
        {
            var list = _fixture.Repository.GetAllDrinks();
            var favouriteService = new DrinkFavouriteService();
            var drink = list.First(x => x.Name.Contains("Cuba Libra"));
            
            favouriteService.MakeItFavourite(drink);
            favouriteService.MakeItNotFavourite(drink);

            drink.IsFavourite.Should().Be(false);
        }
    }
}
