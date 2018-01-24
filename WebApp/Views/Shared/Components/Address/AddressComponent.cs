using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebApp.Models;

namespace WebApp.ViewComponents
{
	[ViewComponent(Name = "Address")]
	public class AddressComponent : ViewComponent
	{
		private readonly IUserService userService;

		public AddressComponent(IUserService userService)
		{
			this.userService = userService;
		}

		public async Task<IViewComponentResult> InvokeAsync(int employeeId)
		{
			var model = new AddressViewModel
			{
				EmployeeId = employeeId,
				Line1 = "Secret Location",
				Line2 = "London",
				Line3 = "UK"
			};
			model.FormattedValue = $"{model.Line1}, {model.Line2}, {model.Line3}";
			return View(model);
		}
	}
}