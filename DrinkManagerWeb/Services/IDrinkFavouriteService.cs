using BLL;

namespace DrinkManagerWeb.Services
{
    interface IDrinkFavouriteService
    {
        Drink MakeItFavourite(Drink drink);
        Drink MakeItNotFavourite(Drink drink);
    }
}
