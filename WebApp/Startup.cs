using System;
using System.Threading.Tasks;
using Castle.Facilities.AspNetCore;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace WebApp
{
	public class Startup
	{
		private readonly WindsorContainer container = new WindsorContainer();

		public Startup(IHostingEnvironment env)
		{
			var builder = new ConfigurationBuilder()
				.SetBasePath(env.ContentRootPath)
				.AddJsonFile("appsettings.json", true, true)
				.AddJsonFile($"appsettings.{env.EnvironmentName}.json", true)
				.AddEnvironmentVariables();

			Configuration = builder.Build();
		}

		public IConfigurationRoot Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{	
			// Add framework services.
			services.AddMvc();
			services.AddLogging();
			services.AddTransient<IOpenGenericService<ClosedGenericTypeParameter>, OpenGenericService<ClosedGenericTypeParameter>>();
			services.AddCastleWindsor(container);
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
		{
			app.UseCastleWindsor<Startup>(container);

			RegisterApplicationComponents();

			// Add custom middleware
			app.UseCastleWindsorMiddleware<CustomMiddleware>(container);

			app.UseStaticFiles();

			app.UseMvc(routes =>
			{
				routes.MapRoute(
					"default",
					"{controller=Home}/{action=Index}/{id?}");
			});
		}

		private void RegisterApplicationComponents()
		{
			// Custom registrations
			container.Register(Component.For<IUserService>().ImplementedBy<AspNetUserService>().LifestyleScoped());
		}
	}

	public interface IUserService
	{
	}

	public class AspNetUserService : IUserService
	{
	}

	// Example of some custom user-defined middleware component.
	public sealed class CustomMiddleware : ICastleWindsorMiddleware
	{
		private readonly IUserService userService;

		public CustomMiddleware(ILoggerFactory loggerFactory, IUserService userService)
		{
			this.userService = userService;
		}

		public async Task Invoke(HttpContext context, Func<Task> next)
		{
			// Do something before
			await next();
			// Do something after
		}
	}

	public interface IOpenGenericService<T> where T : ClosedGenericTypeParameter { }
	public class OpenGenericService<T> : IOpenGenericService<T> where T: ClosedGenericTypeParameter { }
	public class ClosedGenericTypeParameter { }
}