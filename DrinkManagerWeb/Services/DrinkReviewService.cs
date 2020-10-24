using BLL;
using System;

namespace DrinkManagerWeb.Services
{
    public class DrinkReviewService : IDrinkReviewService
    {
        public Drink AddReview(string reviewText, int reviewScore, Drink drinkToUpdate)
        {
            drinkToUpdate.DrinkReview = new DrinkReview
            {
                ReviewText = reviewText,
                ReviewScore = reviewScore
            };

            drinkToUpdate.IsReviewed = true;
            drinkToUpdate.DrinkReview.ReviewDate = DateTime.Now;
            return drinkToUpdate;
        }
    }
}
