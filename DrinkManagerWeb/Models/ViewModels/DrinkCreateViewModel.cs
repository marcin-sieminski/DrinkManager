using BLL;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DrinkManagerWeb.Models.ViewModels
{
    public class DrinkCreateViewModel
    {
        public string Id { get; set; }

        [Required]
        [StringLength(25)]
        public string Name { get; set; }
        [Required]
        [StringLength(25)]
        public string Category { get; set; }
        public string AlcoholicInfo { get; set; }
        [Required]
        [StringLength(25)]
        public string GlassType { get; set; }
        [Required]
        public string Instructions { get; set; }
        [DisplayName("Image URL")]
        [Url]
        public string ImageUrl { get; set; }
        [DisplayName("Ingredient")]
        [Required(ErrorMessage = "Ingredient is required.")]
        [StringLength(25)]
        public string Ingredient1 { get; set; }
        [DisplayName("Amount")]
        [Required(ErrorMessage = "Amount is required.")]
        [StringLength(25)]
        public string Amount1 { get; set; }
        public string Ingredient2 { get; set; }
        public string Amount2 { get; set; }
        public string Ingredient3 { get; set; }
        public string Amount3 { get; set; }
        public string Ingredient4 { get; set; }
        public string Amount4 { get; set; }
        public string Ingredient5 { get; set; }
        public string Amount5 { get; set; }
        public string Ingredient6 { get; set; }
        public string Amount6 { get; set; }
        public string Ingredient7 { get; set; }
        public string Amount7 { get; set; }
        public string Ingredient8 { get; set; }
        public string Amount8 { get; set; }
        public string Ingredient9 { get; set; }
        public string Amount9 { get; set; }
        public string Ingredient10 { get; set; }
        public string Amount10 { get; set; }
        public string Ingredient11 { get; set; }
        public string Amount11 { get; set; }
        public string Ingredient12 { get; set; }
        public string Amount12 { get; set; }
        public string Ingredient13 { get; set; }
        public string Amount13 { get; set; }
        public string Ingredient14 { get; set; }
        public string Amount14 { get; set; }
        public string Ingredient15 { get; set; }
        public string Amount15 { get; set; }

        public List<Ingredient> Ingredients { get; set; }

        public DrinkReview DrinkReview { get; set; }
    }
}
