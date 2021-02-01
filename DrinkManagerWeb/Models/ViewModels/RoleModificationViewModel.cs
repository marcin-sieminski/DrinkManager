using System.ComponentModel.DataAnnotations;

namespace DrinkManagerWeb.Models.ViewModels
{
    public class RoleModificationViewModel
    {
        [Required]
        public string RoleName { get; set; }
 
        public string RoleId { get; set; }
 
        public string[] AddIds { get; set; }
 
        public string[] DeleteIds { get; set; }
    }
}
