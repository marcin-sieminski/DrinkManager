using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace BLL
{
    public class AppUser : IdentityUser
    {
        public List<DrinkReview> Reviews { get; set; }
        public List<UserDrink> UserDrinks { get; set; }
    }
}