using System.Linq;
using Castle.MicroKernel.Lifestyle;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Castle.Facilities.AspNetCore
{
	public static class CastleWindsorRegistrationExtensions
    {
	    public static void AddCastleWindsor(this IServiceCollection services, IWindsorContainer container)
	    {
		    services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
		    services.AddRequestScopingMiddleware(container.BeginScope);
		    services.AddCustomControllerActivation(container.Resolve);
		    container.Kernel.Resolver.AddSubResolver(new FrameworkConfigurationDependencyResolver(services));
		}

	    public static void UseCastleWindsor(this IApplicationBuilder app, IWindsorContainer container)
	    {
		    var controllerTypes = app.GetControllerTypes();
		    if (controllerTypes.Any())
			    container.Register(Component.For(controllerTypes).LifestyleScoped());

		    var viewComponentTypes = app.GetViewComponentTypes();
		    if (viewComponentTypes.Any())
			    container.Register(Component.For(viewComponentTypes));
		}

	    public static void UseCastleWindsorMiddleware<T>(this IApplicationBuilder app, IWindsorContainer container) where T: class, ICastleWindsorMiddleware
	    {
		    container.Register(Component.For<T>());
		    app.Use(async (context, next) => { await container.Resolve<T>().Invoke(context, next); });
		}
	}
}
