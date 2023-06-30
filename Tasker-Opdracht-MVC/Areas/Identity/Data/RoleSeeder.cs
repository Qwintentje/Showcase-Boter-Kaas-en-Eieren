using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tasker_Opdracht_MVC.Data.Entities;

namespace Tasker_Opdracht_MVC.Areas.Identity.Data
{
	public static class RoleSeeder
	{
		public static void SeedRoles(this ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<IdentityRole>().HasData(
				new IdentityRole
				{
					Id = "dcd6d563-6d19-48ef-aa6c-5d95b9b7450e",
					Name = "administrator",
					NormalizedName = "ADMINISTRATOR"
				},
				new IdentityRole
				{
					Id = "873e0bfe-6e1d-4b75-a3c3-30a107c8f2fc",
					Name = "manager",
					NormalizedName = "MANAGER"
				});
		}
	}
}
