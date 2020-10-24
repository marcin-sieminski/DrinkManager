using BLL;

namespace DrinkManagerWeb.Services
{
    public interface IDrinkReviewService
    {
        Drink AddReview(string reviewText, int reviewScore, Drink drinkToUpdate);
    }
}
