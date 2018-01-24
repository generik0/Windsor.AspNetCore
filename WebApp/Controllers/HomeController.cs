using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures.Internal;
using Microsoft.Extensions.Logging;
using WebApp.Models;

namespace WebApp.Controllers
{
    public class HomeController : Controller
    {
	    private readonly IUserService userService;
	    private readonly ILoggerFactory loggerFactory;
	    private readonly IViewBufferScope viewBufferScope;

	    public HomeController(IUserService userService, ILoggerFactory loggerFactory, IViewBufferScope viewBufferScope)
	    {
		    this.userService = userService;
		    this.loggerFactory = loggerFactory;
		    this.viewBufferScope = viewBufferScope;
	    }

	    public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
