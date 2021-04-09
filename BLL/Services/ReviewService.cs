using System;

namespace BLL.Services
{
    public class ReviewService
    {
        public DrinkReview AddReview(int score, string review)
        {
            return new DrinkReview
            {
                ReviewScore = score,
                ReviewText = review,
                ReviewDate = DateTime.Now
            };
        }
    }
}