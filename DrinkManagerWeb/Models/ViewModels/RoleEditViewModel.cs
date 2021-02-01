using BLL;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace DrinkManagerWeb.Models.ViewModels
{
    public class RoleEditViewModel
    {
        public IdentityRole Role { get; set; }
        public IEnumerable<AppUser> Members { get; set; }
        public IEnumerable<AppUser> NonMembers { get; set; }
    }
}
