#nullable enable
using BLL;
using BLL.Data.Repositories;
using BLL.Enums;
using BLL.Services;
using DrinkManagerWeb.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
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
        private readonly IReportingApiService _reportingApiService;
        private readonly IFavouriteRepository _favouriteRepository;
        private readonly IReviewRepository _reviewRepository;
        private readonly UserManager<AppUser> _userManager;
        private readonly int _pageSize = 12;

        public DrinksController(
            IDrinkRepository drinkRepository,
            IDrinkSearchService drinkSearchService,
            IFavouriteRepository favouriteRepository,
            IReviewRepository reviewRepository,
            UserManager<AppUser> userManager,
            IReportingApiService reportingApiService)
        {
            _drinkRepository = drinkRepository;
            _drinkSearchService = drinkSearchService;
            _favouriteRepository = favouriteRepository;
            _reviewRepository = reviewRepository;
            _userManager = userManager;
            _reportingApiService = reportingApiService;
        }

        public IActionResult Index(int? pageNumber)
        {
            Task.Run(() => _reportingApiService.UserDidSomething(PerformedAction.AllDrinks, this.User.Identity.Name, drinkId: null, searchedPhrase: null, score: null));
            var drinks = _drinkRepository.GetAllDrinks().OrderBy(x => x.Name);
            var model = new DrinksViewModel
            {
                Drinks = PaginatedList<Drink>.CreatePaginatedList(drinks, pageNumber ?? 1, _pageSize)
            };
            return View(model);
        }

        [HttpGet("drink/{id}")]
        public async Task<IActionResult> DrinkDetails(string id)
        {
            Task.Run(() => 
                _reportingApiService.UserDidSomething(PerformedAction.CheckedDrinkDetails, this.User.Identity.Name, id, searchedPhrase: null, score: null));
            var drink = await _drinkRepository.GetDrinkById(id);
            if (drink == null)
            {
                // add error View
            }

            var model = new DrinkDetailsViewModel
            {
                Drink = drink,
                IsFavourite = _favouriteRepository.IsFavourite(_userManager.GetUserId(User), drink?.DrinkId),
                CanUserReview = _reviewRepository.CanUserReviewDrink(_userManager.GetUserId(User), drink?.DrinkId)
            };
            return View(model);
        }

        [Authorize]
        [HttpGet("Drinks/favourites")]
        public IActionResult FavouriteDrinks(int? pageNumber)
        {
            var drinks = _favouriteRepository.GetUserFavouriteDrinks(_userManager.GetUserId(User));

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
            Task.Run(() => 
                _reportingApiService.UserDidSomething(PerformedAction.EditOrCreateDrink, this.User.Identity.Name, id, searchedPhrase: null, score: null));
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
                    TempData["Alert"] = "Drink not found";
                    TempData["AlertClass"] = "alert-danger";

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
                    DrinkReviews = new List<DrinkReview>(),
                    Category = data["Category"],
                    AlcoholicInfo = data["AlcoholicInfo"],
                    Instructions = data["Instructions"],
                    Name = data["Name"]
                };

                await _drinkRepository.AddDrink(newDrink);
                redirectId = newDrink.DrinkId;
            }

            await _drinkRepository.SaveChanges();

            return RedirectToAction(nameof(DrinkDetails), new { id = redirectId });
        }


        public async Task<IActionResult> Remove(string id)
        {
            Task.Run(() => 
                _reportingApiService.UserDidSomething(PerformedAction.RemoveDrink, this.User.Identity.Name, id, searchedPhrase: null, score: null));
            var drink = await _drinkRepository.GetDrinkById(id);

            if (drink == null)
            {
                return NotFound();
            }

            _drinkRepository.DeleteDrink(drink);
            await _drinkRepository.SaveChanges();

            TempData["Alert"] = $"Drink {drink.Name} removed";
            TempData["AlertClass"] = "alert-success";

            return RedirectToAction(nameof(Index));
        }

        [Authorize]
        public async Task<IActionResult> AddToFavourite(string id)
        {
            Task.Run(() =>
                _reportingApiService.UserDidSomething(PerformedAction.AddedToFavourite, this.User.Identity.Name, id, searchedPhrase: null, score: null));
            var drink = await _drinkRepository.GetDrinkById(id);
            if (drink == null)
            {
                // add error View
            }

            _favouriteRepository.AddToFavourites(_userManager.GetUserId(User), drink);

            return RedirectToAction("DrinkDetails", new { id });
        }

        [Authorize]
        public async Task<IActionResult> RemoveFromFavourite(string id)
        {
            Task.Run(() =>
                _reportingApiService.UserDidSomething(PerformedAction.RemovedFromFavourite, this.User.Identity.Name, id, searchedPhrase: null, score: null));
            var drink = await _drinkRepository.GetDrinkById(id);
            if (drink == null)
            {
                // add error View
            }

            _favouriteRepository.RemoveFromFavourites(_userManager.GetUserId(User), drink?.DrinkId);

            return RedirectToAction("DrinkDetails", new { id });
        }

        [Authorize]
        [HttpGet("drink/addReview")]
        public async Task<IActionResult> AddReview(string? id)
        {
            var drink = await _drinkRepository.GetDrinkById(id);

            var model = new DrinkCreateViewModel
            {
                Name = drink?.Name,
                Id = drink?.DrinkId
            };

            return View("AddReview", model);
        }

        [HttpPost("drink/addReview")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddReview(IFormCollection data, string? id)
        {
            Task.Run(() =>
                _reportingApiService.UserDidSomething(PerformedAction.AddedReview, this.User.Identity.Name, id, searchedPhrase: null, int.Parse(data["DrinkReview.ReviewScore"])));
            var drinkToUpdate = await _drinkRepository.GetDrinkById(id);

            if (drinkToUpdate == null)
            {
                TempData["Alert"] = "Drink not found.";
                TempData["AlertClass"] = "alert-danger";
                return RedirectToAction(nameof(Index));
            }

            drinkToUpdate.DrinkReviews.Add(new DrinkReview
            {
                ReviewText = data["DrinkReview.ReviewText"],
                ReviewScore = int.Parse(data["DrinkReview.ReviewScore"]),
                Drink = drinkToUpdate,
                Author = await _userManager.GetUserAsync(User),
                ReviewDate = DateTime.Now
            });
            _drinkRepository.Update(drinkToUpdate);
            await _drinkRepository.SaveChanges();

            return RedirectToAction(nameof(DrinkDetails), new { id });
        }

        [Authorize]
        [HttpGet("Drinks/reviews")]
        public IActionResult ReviewedDrinks(int? pageNumber)
        {
            var drinks = _reviewRepository.GetUserReviewedDrinks(_userManager.GetUserId(User));

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
            var drinks = _drinkSearchService
                .SearchByAlcoholContent(alcoholics, nonAlcoholics, optionalAlcoholics);

            //Model saves alcoholic content info passed by controller to save
            //user choices while going through PaginatedList pages
            var model = new DrinksViewModel
            {
                Drinks = PaginatedList<Drink>.CreatePaginatedList(drinks, pageNumber ?? 1, _pageSize),
                Alcoholics = alcoholics,
                NonAlcoholics = nonAlcoholics,
                OptionalAlcoholics = optionalAlcoholics
            };
            return View(model);
        }

        public IActionResult SearchByName(string searchString, int? pageNumber)
        {
            var drinks = _drinkRepository.GetAllDrinks();
            if (!string.IsNullOrEmpty(searchString))
            {
                Task.Run(() =>
                    _reportingApiService.UserDidSomething(PerformedAction.SearchByName, this.User.Identity.Name, drinkId: null, searchString, score: null));
                drinks = _drinkSearchService.SearchByName(searchString);
            }

            ViewData["SearchString"] = searchString;
            ViewData["SearchType"] = "SearchByName";

            var model = new DrinksViewModel
            {
                Drinks = PaginatedList<Drink>.CreatePaginatedList(drinks.AsEnumerable(),
                    pageNumber ?? 1, _pageSize)
            };
            return View(model);
        }

        public IActionResult SearchByIngredients(string searchString, int? pageNumber,
            string searchCondition = "any")
        {
            var drinks = _drinkRepository.GetAllDrinks();

            if (!string.IsNullOrEmpty(searchString))
            {
                Task.Run(() =>
                    _reportingApiService.UserDidSomething(PerformedAction.SearchByIngredients, this.User.Identity.Name, drinkId: null, searchString, score: null));
                var searchDrinkIngredientsCondition =
                    searchCondition.Equals("all") ? SearchDrinkOption.All : SearchDrinkOption.Any;
                drinks = _drinkSearchService.SearchByIngredients(new SortedSet<string>(searchString.Split(' ')),
                    searchDrinkIngredientsCondition);
            }

            ViewData["SearchString"] = searchString;
            ViewData["SearchCondition"] = searchCondition;
            ViewData["SearchType"] = "SearchByIngredients";


            var model = new DrinksViewModel
            {
                Drinks = PaginatedList<Drink>.CreatePaginatedList(drinks,
                    pageNumber ?? 1, _pageSize)
            };
            return View(model);
        }
    }
}