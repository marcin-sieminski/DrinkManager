using DrinkManagerWeb.Services;
using FluentAssertions;
using System.Linq;
using Xunit;

namespace Tests
{
    public class DrinkReviewServiceShould : IClassFixture<TestWithSqlite>
    {
        private readonly TestWithSqlite _fixture;

        public DrinkReviewServiceShould(TestWithSqlite fixture)
        {
            _fixture = fixture;
        }
        [Fact]
        public void AddReviewToDrink()
        {
            var list = _fixture.Repository.GetAllDrinks();
            var reviewService = new DrinkReviewService();
            var drink = list.First(x => x.Name == "Cuba Libra");

            reviewService.AddReview("Cool", 4, drink);

            drink.DrinkReview.Should().NotBe(null);
        }
        [Fact]
        public void ChangeReviewEvenThoughItShouldNot()
        {
            var list = _fixture.Repository.GetAllDrinks();
            var reviewService = new DrinkReviewService();
            var drink = list.First(x => x.Name == "Cuba Libre");

            reviewService.AddReview("Cool", 4, drink);
            var savedScore = drink.DrinkReview.ReviewScore;
            reviewService.AddReview("Very Good", 5, drink);

            drink.DrinkReview.ReviewScore.Should().NotBe(savedScore);
        }
    }
}
