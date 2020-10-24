using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BLL
{
    public class Drink
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string DrinkId { get; set; }

        [Required]
        public string Name { get; set; }

        public string Category { get; set; }
        public string AlcoholicInfo { get; set; }
        public string GlassType { get; set; }
        public string Instructions { get; set; }
        public string ImageUrl { get; set; }
        public List<Ingredient> Ingredients { get; set; }
        public DrinkReview DrinkReview { get; set; }
        public bool IsReviewed { get; set; }
        public bool IsFavourite { get; set; }
    }
}