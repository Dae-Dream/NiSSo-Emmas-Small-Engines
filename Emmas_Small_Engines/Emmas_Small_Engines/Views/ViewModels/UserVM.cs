using System.ComponentModel.DataAnnotations;

namespace Emmas_Small_Engines.Views.ViewModels
{
	public class UserVM
	{
		public string Id { get; set; }

		[Display(Name = "User Name")]
		public string UserName { get; set; }

		[Display(Name = "Roles")]
		public List<String> UserRoles { get; set; }
	}
}
