using BLL.Data.Repositories;
using DrinkManagerWeb.Services;
using FluentAssertions;
using System.Linq;
using Xunit;

namespace Tests
{
    public class DrinkFavouriteServiceShould
    {
        [Fact]
        public void ChangeIsFavouriteToTrueIfMakeItFavouriteWasCalled()
        {
            var list = new TestDrinkRepository().GetTestDrinks();
            var favouriteService = new DrinkFavouriteService();
            
            favouriteService.MakeItFavourite(list.First());

            list.First().IsFavourite.Should().Be(true);
        }
        [Fact]
        public void ChangeIsFavouriteToFalseIfMakeItNotFavouriteWasCalled()
        {
            var list = new TestDrinkRepository().GetTestDrinks();
            var favouriteService = new DrinkFavouriteService();
            favouriteService.MakeItFavourite(list.First());

            favouriteService.MakeItNotFavourite(list.First());

            list.First().IsFavourite.Should().Be(false);

        }
    }
}
