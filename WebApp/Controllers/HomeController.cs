using System;
using System.Diagnostics;
using Microsoft.AspNetCore.Http;
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
	    private readonly IHttpContextAccessor accessor;
	    private readonly ILogger<HomeController> logger;
	    private readonly IViewBufferScope viewBufferScope;
	    private readonly IOpenGenericService<ClosedGenericTypeParameter> closedGenericService;

	    public HomeController(IHttpContextAccessor accessor, IUserService userService, ILoggerFactory loggerFactory, ILogger<HomeController> logger, IViewBufferScope viewBufferScope, IOpenGenericService<ClosedGenericTypeParameter> closedGenericService)
	    {
		    this.accessor = accessor ?? throw new ArgumentNullException(nameof(accessor));
			this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
		    this.userService = userService ?? throw new ArgumentNullException(nameof(userService));
		    this.loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
		    this.viewBufferScope = viewBufferScope ?? throw new ArgumentNullException(nameof(viewBufferScope));
		    this.closedGenericService = closedGenericService;
	    }

	    public IActionResult Index()
	    {
		    var request = accessor.HttpContext.Request;

		    if (accessor.HttpContext.Items["Foo"] == null)
			    accessor.HttpContext.Items["Foo"] = "Testing";

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
