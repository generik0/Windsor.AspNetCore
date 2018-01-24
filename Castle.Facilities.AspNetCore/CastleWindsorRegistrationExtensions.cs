using Castle.MicroKernel.Lifestyle;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Razor.TagHelpers;
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
			services.AddCustomTagHelperActivation(container.Resolve);
			services.AddCustomViewComponentActivation(container.Resolve);
			container.Kernel.Resolver.AddSubResolver(new FrameworkConfigurationDependencyResolver(services));
		}

		public static void UseCastleWindsor<TStartup>(this IApplicationBuilder app, IWindsorContainer container)
		{
			container.Register(Classes.FromAssemblyInThisApplication(typeof(TStartup).Assembly).BasedOn<Controller>().LifestyleScoped());
			container.Register(Classes.FromAssemblyInThisApplication(typeof(TStartup).Assembly).BasedOn<ViewComponent>().LifestyleScoped());
			container.Register(Classes.FromAssemblyInThisApplication(typeof(TStartup).Assembly).BasedOn<TagHelper>().LifestyleScoped());
		}

		public static void UseCastleWindsorMiddleware<T>(this IApplicationBuilder app, IWindsorContainer container) where T : class, ICastleWindsorMiddleware
		{
			container.Register(Component.For<T>());
			app.Use(async (context, next) => { await container.Resolve<T>().Invoke(context, next); });
		}
	}
}