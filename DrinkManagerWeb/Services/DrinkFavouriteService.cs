using BLL;

namespace DrinkManagerWeb.Services
{
    public class DrinkFavouriteService : IDrinkFavouriteService
    {
        public Drink MakeItFavourite(Drink drink)
        {
            drink.IsFavourite = true;
            return drink;
        }
        public Drink MakeItNotFavourite(Drink drink)
        {
            drink.IsFavourite = false;
            return drink;
        }
    }
}
