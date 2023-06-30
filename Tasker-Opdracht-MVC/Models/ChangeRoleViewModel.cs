using Microsoft.AspNetCore.Identity;
using Tasker_Opdracht_MVC.Areas.Identity.Data;

namespace Tasker_Opdracht_MVC.Models
{
    internal class ChangeRoleViewModel
    {
        public List<ApplicationUser> Users { get; set; }
        public List<IdentityRole> Roles { get; set; }
    }
}