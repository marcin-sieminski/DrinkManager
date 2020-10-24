using DrinkManagerWeb.Services;
using FluentAssertions;
using System.Linq;
using Xunit;

namespace Tests
{
    public class DrinkReviewServiceShould
    {
        [Fact]
        public void AddReviewToDrink()
        {
            var drinkList = new SampleDrinkCreator().GetTestDrinks();
            var reviewService = new DrinkReviewService();
            var drink = drinkList.First(x => x.DrinkId == "1");

            reviewService.AddReview("Cool", 4, drink);

            drink.DrinkReview.Should().NotBe(null);
        }
        [Fact]
        public void ChangeReviewEvenThoughItShouldNot()
        {
            var drinkList = new SampleDrinkCreator().GetTestDrinks();
            var reviewService = new DrinkReviewService();
            var drink = drinkList.First(x => x.DrinkId == "1");
            var savedScore = drink.DrinkReview.ReviewScore;

            reviewService.AddReview("Very Good", 5, drink);

            drink.DrinkReview.ReviewScore.Should().NotBe(savedScore);
        }
    }
}
