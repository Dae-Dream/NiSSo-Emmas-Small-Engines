using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace Emmas_Small_Engines.Data
{
	public static class ApplicationDbInitializer
	{
		public static async void Seed(IApplicationBuilder applicationBuilder)
		{
			ApplicationDbContext context = applicationBuilder.ApplicationServices.CreateScope()
				.ServiceProvider.GetRequiredService<ApplicationDbContext>();

			EmmaSmallEngineContext _context = applicationBuilder.ApplicationServices.CreateScope()
				.ServiceProvider.GetRequiredService<EmmaSmallEngineContext>();

			try
			{
				////Delete the database if you need to apply a new Migration
				//context.Database.EnsureDeleted();
				//Create the database if it does not exist and apply the Migration
				context.Database.Migrate();

				//Create Roles
				var RoleManager = applicationBuilder.ApplicationServices.CreateScope()
					.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
				string[] roleNames = { "Owner", "Admin", "Sales", "Technician", "Ordering" };
				IdentityResult roleResult;
				foreach (var roleName in roleNames)
				{
					var roleExist = await RoleManager.RoleExistsAsync(roleName);
					if (!roleExist)
					{
						roleResult = await RoleManager.CreateAsync(new IdentityRole(roleName));
					}
				}
				//Create Users
				var userManager = applicationBuilder.ApplicationServices.CreateScope()
					.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
				if (userManager.FindByEmailAsync("owner1@outlook.com").Result == null)
				{
					IdentityUser user = new IdentityUser
					{
						UserName = "owner1@outlook.com",
						Email = "owner1@outlook.com"
					};

					IdentityResult result = userManager.CreateAsync(user, "Password1").Result;

					if (result.Succeeded)
					{
						userManager.AddToRoleAsync(user, "Owner").Wait();
					}
				}
				if (userManager.FindByEmailAsync("admin1@outlook.com").Result == null)
				{
					IdentityUser user = new IdentityUser
					{
						UserName = "admin1@outlook.com",
						Email = "admin1@outlook.com"
					};

					IdentityResult result = userManager.CreateAsync(user, "Password1").Result;

					if (result.Succeeded)
					{
						userManager.AddToRoleAsync(user, "Admin").Wait();
					}
				}
				if (userManager.FindByEmailAsync("sales1@outlook.com").Result == null)
				{
					IdentityUser user = new IdentityUser
					{
						UserName = "sales1@outlook.com",
						Email = "sales1@outlook.com"
					};

					IdentityResult result = userManager.CreateAsync(user, "Password1").Result;

					if (result.Succeeded)
					{
						userManager.AddToRoleAsync(user, "Sales").Wait();
					}
				}
				if (userManager.FindByEmailAsync("technician1@outlook.com").Result == null)
				{
					IdentityUser user = new IdentityUser
					{
						UserName = "technician1@outlook.com",
						Email = "technician1@outlook.com"
					};

					IdentityResult result = userManager.CreateAsync(user, "Password1").Result;

					if (result.Succeeded)
					{
						userManager.AddToRoleAsync(user, "Technician").Wait();
					}
				}
				if (userManager.FindByEmailAsync("ordering1@outlook.com").Result == null)
				{
					IdentityUser user = new IdentityUser
					{
						UserName = "ordering1@outlook.com",
						Email = "ordering1@outlook.com"
					};

					IdentityResult result = userManager.CreateAsync(user, "Password1").Result;

					if (result.Succeeded)
					{
						userManager.AddToRoleAsync(user, "Ordering").Wait();
					}
				}
				if (userManager.FindByEmailAsync("user@outlook.com").Result == null)
				{
					IdentityUser user = new IdentityUser
					{
						UserName = "user@outlook.com",
						Email = "user@outlook.com"
					};

					IdentityResult result = userManager.CreateAsync(user, "Password1").Result;
					//Not in any role
				}

				foreach (var emp in _context.Employees)
				{
					if (userManager.FindByEmailAsync(emp.UserName).Result == null)
					{
						IdentityUser user = new IdentityUser
						{
							UserName = emp.UserName,
							Email = emp.UserName
						};

						IdentityResult result = userManager.CreateAsync(user, emp.Password).Result;

						if (result.Succeeded)
						{
							foreach (var pos in emp.EmployeePositions)
							{
								userManager.AddToRoleAsync(user, pos.ToString()).Wait();
							}
						}
					}
				}

			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.GetBaseException().Message);
			}
		}
	}
}
