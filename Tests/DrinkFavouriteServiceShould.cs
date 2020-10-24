using BLL;
using DrinkManagerWeb.Services;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Tests
{
    public class DrinkFavouriteServiceShould
    {
        [Fact]
        public void ChangeIsFavouriteToTrueIfMakeItFavouriteWasCalled()
        {
            var list = new SampleDrinkCreator().GetTestDrinks();
            var favouriteService = new DrinkFavouriteService();
            
            favouriteService.MakeItFavourite(list.First());

            list.First().IsFavourite.Should().Be(true);
        }
        [Fact]
        public void ChangeIsFavouriteToFalseIfMakeItNotFavouriteWasCalled()
        {
            var list = new SampleDrinkCreator().GetTestDrinks();
            var favouriteService = new DrinkFavouriteService();
            favouriteService.MakeItFavourite(list.First());

            favouriteService.MakeItNotFavourite(list.First());

            list.First().IsFavourite.Should().Be(false);

        }
    }

    public class SampleDrinkCreator
    {
        public List<Drink> GetTestDrinks()
        {
            var drinkList = new List<Drink>()
            {
                new Drink()
                {
                    AlcoholicInfo = "Alcoholic",
                    Category = "Special",
                    DrinkId = "1",
                    DrinkReview = new DrinkReview()
                    {
                        Id = 1,
                        ReviewDate = DateTime.Now,
                        ReviewScore = 4,
                        ReviewText = "Cool"
                    },
                    GlassType = "Regular",
                    ImageUrl = "",
                    Ingredients = new List<Ingredient>()
                    {
                        new Ingredient()
                        {
                            Amount = "300ml",
                            DrinkId = "1",
                            IngredientId = "1",
                            Name = "Piccolo"
                        },
                        new Ingredient()
                        {
                            Amount = "Until Full",
                            DrinkId = "1",
                            IngredientId = "2",
                            Name = "Vodka"
                        }
                    },
                    Instructions = "Stir stuff",
                    IsFavourite = false,
                    Name = "Turbo Piccolo"
                },
                new Drink()
                {
                    AlcoholicInfo = "Non Alcoholic",
                    Category = "Not special",
                    DrinkId = "2",
                    DrinkReview = null,
                    GlassType = "Irregular",
                    ImageUrl = "",
                    Ingredients = new List<Ingredient>()
                    {
                        new Ingredient()
                        {
                            Amount = "200ml",
                            DrinkId = "2",
                            IngredientId = "3",
                            Name = "Rum"
                        },
                        new Ingredient()
                        {
                            Amount = "Until Full",
                            DrinkId = "2",
                            IngredientId = "2",
                            Name = "Vodka"
                        }
                    },
                    Instructions = "Stir stuff",
                    IsFavourite = false,
                    Name = "Turbo Rum"
                }
            };
            return drinkList;
        }
    }
}
