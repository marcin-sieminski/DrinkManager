using BLL;

namespace DrinkManagerWeb.Services
{
    public interface IDrinkFavouriteService
    {
        Drink MakeItFavourite(Drink drink);
        Drink MakeItNotFavourite(Drink drink);
    }
}
