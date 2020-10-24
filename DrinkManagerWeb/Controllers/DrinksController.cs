#nullable enable
using BLL;
using BLL.Data.Repositories;
using BLL.Enums;
using DrinkManagerWeb.Models.ViewModels;
using DrinkManagerWeb.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DrinkManagerWeb.Controllers
{
    public class DrinksController : Controller
    {
        private readonly IDrinkRepository _drinkRepository;
        private readonly IDrinkSearchService _drinkSearchService;
        private readonly IDrinkFavouriteService _drinkFavouriteService;
        private readonly int _pageSize = 12;

        public DrinksController(IDrinkRepository drinkRepository, IDrinkSearchService drinkSearchService)
        {
            _drinkRepository = drinkRepository;
            _drinkSearchService = drinkSearchService;
        }

        public IActionResult Index(string sortOrder, int? pageNumber)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["NameSortParm"] = string.IsNullOrEmpty(sortOrder) ? "name_desc" : "";

            var drinks = _drinkRepository.GetAllDrinks().AsEnumerable();

            drinks = sortOrder switch
            {
                "name_desc" => drinks.OrderByDescending(s => s.Name),
                _ => drinks.OrderBy(s => s.Name),
            };
            var model = new DrinksViewModel
            {
                Drinks = PaginatedList<Drink>.CreatePaginatedList(drinks, pageNumber ?? 1, _pageSize)
            };
            return View(model);
        }

        [HttpGet("drink/{id}")]
        public async Task<IActionResult> DrinkDetails(string id)
        {
            var drink = await _drinkRepository.GetDrinkById(id);
            if (drink == null)
            {
                // add error View
            }

            return View(drink);
        }

        [HttpGet("Drinks/favourites")]
        public IActionResult FavouriteDrinks(string sortOrder, int? pageNumber)
        {
            var drinks = _drinkRepository.GetAllDrinks().Where(x => x.IsFavourite);

            var model = new DrinksViewModel
            {
                Drinks = PaginatedList<Drink>.CreatePaginatedList(drinks, pageNumber ?? 1, _pageSize)
            };
            return View(model);
        }

        [HttpGet("drink/edit/{id}")]
        public async Task<IActionResult> Edit(string? id)
        {
            var drink = await _drinkRepository.GetDrinkById(id);

            var model = new DrinkCreateViewModel
            {
                Id = drink?.DrinkId,
                GlassType = drink?.GlassType,
                Category = drink?.Category,
                Instructions = drink?.Instructions,
                AlcoholicInfo = drink?.AlcoholicInfo,
                Name = drink?.Name,
                Ingredients = drink?.Ingredients,
                ImageUrl = drink?.ImageUrl
            };

            return View("Create", model);
        }

        [HttpGet("drink/create")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost("drink/create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(IFormCollection data, string? id)
        {

            if (!ModelState.IsValid)
            {
                return View();
            }

            var ingredients = new List<Ingredient>();

            // create ingredient objects from the from data
            foreach (var key in data.Keys)
            {
                if (key.Contains("Ingredient"))
                {
                    ingredients.Add(new Ingredient
                    {
                        Name = data[key],
                        Amount = data["Amount" + key.Split("Ingredient")[1]]
                    });
                }
            }

            // image placeholder
            var imageUrl = "https://medifactia.com/wp-content/uploads/2018/01/placeholder.png";

            // if image data exists replace placeholder
            if (data.ContainsKey("ImageUrl") && string.IsNullOrWhiteSpace(data["ImageUrl"]) == false)
            {
                imageUrl = data["ImageUrl"];
            }

            // id that we will use for the redirect
            string redirectId;

            if (id != null)
            {
                // ID is not null, we edit
                var drinkToUpdate = await _drinkRepository.GetDrinkById(id);


                if (drinkToUpdate == null)
                {
                    // something went wrong redirect to drinks index
                    TempData["Alert"] = $"Drink not found";
                    return RedirectToAction(nameof(Index));
                }

                drinkToUpdate.Ingredients = ingredients;
                drinkToUpdate.GlassType = data["GlassType"];
                drinkToUpdate.Category = data["Category"];
                drinkToUpdate.AlcoholicInfo = data["AlcoholicInfo"];
                drinkToUpdate.Instructions = data["Instructions"];
                drinkToUpdate.Name = data["Name"];
                drinkToUpdate.ImageUrl = imageUrl;
                _drinkRepository.Update(drinkToUpdate);
                redirectId = id;
            }
            else
            {
                // id was null, we create a new drink
                var newDrink = new Drink
                {
                    DrinkId = Guid.NewGuid().ToString(),
                    Ingredients = ingredients,
                    GlassType = data["GlassType"],
                    ImageUrl = imageUrl,
                    DrinkReview = null,
                    Category = data["Category"],
                    AlcoholicInfo = data["AlcoholicInfo"],
                    Instructions = data["Instructions"],
                    Name = data["Name"]
                };

                await _drinkRepository.AddDrink(newDrink);
                redirectId = newDrink.DrinkId;
            }

            await _drinkRepository.SaveChanges();

            return RedirectToAction(nameof(DrinkDetails), new {id = redirectId});
        }

        public async Task<IActionResult> Remove(string id)
        {
            var drink = await _drinkRepository.GetDrinkById(id);

            if (drink == null)
            {
                return NotFound();
            }

            _drinkRepository.DeleteDrink(drink);
            await _drinkRepository.SaveChanges();

            TempData["Alert"] = $"Drink {drink.Name} removed";

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> AddToFavourite(string id)
        {
            var drink = await _drinkRepository.GetDrinkById(id);
            if (drink == null)
            {
                // add error View
            }

            drink = _drinkFavouriteService.MakeItFavourite(drink);

            _drinkRepository.Update(drink);
            await _drinkRepository.SaveChanges();

            return RedirectToAction("DrinkDetails", new {id});
        }

        public async Task<IActionResult> RemoveFromFavourite(string id)
        {
            var drink = await _drinkRepository.GetDrinkById(id);
            if (drink == null)
            {
                // add error View
            }
            drink = _drinkFavouriteService.MakeItNotFavourite(drink);

            _drinkRepository.Update(drink);
            await _drinkRepository.SaveChanges();

            return RedirectToAction("DrinkDetails", new {id});
        }

        [HttpGet("drink/addReview")]
        public async Task<IActionResult> AddReview(string? id)
        {
            var drink = await _drinkRepository.GetDrinkById(id);

            var model = new DrinkCreateViewModel
            {
                DrinkReview = drink?.DrinkReview,
                Name = drink?.Name,
                Id = drink?.DrinkId
            };

            return View("AddReview", model);
        }

        [HttpPost("drink/addReview")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddReview(IFormCollection data, string? id)
        {

            var drinkToUpdate = await _drinkRepository.GetDrinkById(id);

            if (drinkToUpdate == null)
            {
                TempData["Alert"] = "Drink not found.";
                return RedirectToAction(nameof(Index));
            }

            drinkToUpdate.DrinkReview = new DrinkReview
            {
                ReviewText = data["DrinkReview.ReviewText"],
                ReviewScore = int.Parse(data["DrinkReview.ReviewScore"])
            };

            drinkToUpdate.IsReviewed = true;
            drinkToUpdate.DrinkReview.ReviewDate = DateTime.Now;

            _drinkRepository.Update(drinkToUpdate);
            await _drinkRepository.SaveChanges();

            return RedirectToAction(nameof(DrinkDetails), new {id});
        }

        [HttpGet("Drinks/reviews")]
        public IActionResult ReviewedDrinks(string sortOrder, int? pageNumber)
        {
            var drinks = _drinkRepository.GetAllDrinks().Where(x => x.IsReviewed);

            var model = new DrinksViewModel
            {
                Drinks = PaginatedList<Drink>.CreatePaginatedList(drinks, pageNumber ?? 1, _pageSize)
            };
            return View(model);
        }

        public IActionResult SearchByAlcoholContent(int? pageNumber, bool alcoholics = true, bool nonAlcoholics = true, bool optionalAlcoholics = true)
        {
            ViewData["Alcoholics"] = alcoholics;
            ViewData["nonAlcoholics"] = nonAlcoholics;
            ViewData["optionalAlcoholics"] = optionalAlcoholics;
            int pageSize = 12;
            var drinks = _drinkSearchService.SearchByAlcoholContent(alcoholics, nonAlcoholics, optionalAlcoholics, _drinkRepository.GetAllDrinks().ToList());

            //Model saves alcoholic content info passed by controller to save
            //user choices while going through PaginatedList pages
            var model = new DrinksViewModel
            {
                Drinks = PaginatedList<Drink>.CreatePaginatedList(drinks, pageNumber ?? 1, pageSize),
                Alcoholics = alcoholics,
                NonAlcoholics = nonAlcoholics,
                OptionalAlcoholics = optionalAlcoholics
            };
            return View(model);
        }

        public IActionResult SearchByName(string searchString, string sortOrder, int? pageNumber)
        {
            var drinks = _drinkRepository.GetAllDrinks();

            if (!string.IsNullOrEmpty(searchString))
            {
                drinks = _drinkSearchService.SearchByName(searchString);
            }

            ViewData["SearchString"] = searchString;
            ViewData["SearchType"] = "SearchByName";
            ViewData["CurrentSort"] = sortOrder;
            ViewData["NameSortParm"] = string.IsNullOrEmpty(sortOrder) ? "name_desc" : "";

            var model = new DrinksViewModel
            {
                Drinks = PaginatedList<Drink>.CreatePaginatedList(_drinkSearchService.SortDrinks(sortOrder, drinks),
                    pageNumber ?? 1, _pageSize)
            };
            return View(model);
        }

        public IActionResult SearchByIngredients(string searchString, string sortOrder, int? pageNumber,
            string searchCondition = "any")
        {
            var drinks = _drinkRepository.GetAllDrinks();

            if (!string.IsNullOrEmpty(searchString))
            {
                var searchDrinkIngredientsCondition =
                    searchCondition.Equals("all") ? SearchDrinkOption.All : SearchDrinkOption.Any;
                drinks = _drinkSearchService.SearchByIngredients(new SortedSet<string>(searchString.Split(' ')),
                    searchDrinkIngredientsCondition);
            }

            ViewData["SearchString"] = searchString;
            ViewData["SearchCondition"] = searchCondition;
            ViewData["SearchType"] = "SearchByIngredients";
            ViewData["CurrentSort"] = sortOrder;
            ViewData["NameSortParm"] = string.IsNullOrEmpty(sortOrder) ? "name_desc" : "";

            var model = new DrinksViewModel
            {
                Drinks = PaginatedList<Drink>.CreatePaginatedList(_drinkSearchService.SortDrinks(sortOrder, drinks),
                    pageNumber ?? 1, _pageSize)
            };
            return View(model);
        }
    }
}