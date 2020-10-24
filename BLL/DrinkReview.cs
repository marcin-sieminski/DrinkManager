using System;
using System.ComponentModel.DataAnnotations;

namespace BLL
{
    public class DrinkReview
    {
        public const int MinScore = 0;
        public const int MaxScore = 5;

        [Key]
        public int Id { get; set; }

        public int ReviewScore { get; set; }

        public string ReviewText { get; set; }

        public DateTime ReviewDate { get; set; }

        public string DrinkId { get; set; }
    }
}