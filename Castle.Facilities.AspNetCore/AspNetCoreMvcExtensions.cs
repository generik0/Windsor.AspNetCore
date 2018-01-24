using System;
using System.Linq;
using System.Reflection;
using Castle.Facilities.AspNetCore.Activators;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Internal;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Razor.Internal;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.Extensions.DependencyInjection;

namespace Castle.Facilities.AspNetCore
{
	public static class AspNetCoreMvcExtensions
	{
		public static void AddCustomControllerActivation(this IServiceCollection services, Func<Type, object> activator)
		{
			if (services == null) throw new ArgumentNullException(nameof(services));
			if (activator == null) throw new ArgumentNullException(nameof(activator));

			services.AddSingleton<IControllerActivator>(new DelegatingControllerActivator(context => activator(context.ActionDescriptor.ControllerTypeInfo.AsType())));
		}

		public static void AddCustomViewComponentActivation(this IServiceCollection services, Func<Type, object> activator)
		{
			if (services == null) throw new ArgumentNullException(nameof(services));
			if (activator == null) throw new ArgumentNullException(nameof(activator));

			services.AddSingleton<IViewComponentActivator>(new DelegatingViewComponentActivator(activator));
		}

		public static void AddCustomTagHelperActivation(this IServiceCollection services, Func<Type, object> activator, Predicate<Type> applicationTypeSelector = null)
		{
			if (services == null) throw new ArgumentNullException(nameof(services));
			if (activator == null) throw new ArgumentNullException(nameof(activator));

			// There are tag helpers OOTB in MVC. Letting the application container try to create them will fail
			// because of the dependencies these tag helpers have. This means that OOTB tag helpers need to remain
			// created by the framework's DefaultTagHelperActivator, hence the selector predicate.
			applicationTypeSelector = applicationTypeSelector ?? (type => !type.GetTypeInfo().Namespace.StartsWith("Microsoft"));

			services.AddSingleton<ITagHelperActivator>(provider =>
				new DelegatingTagHelperActivator(
					customCreatorSelector: applicationTypeSelector,
					customTagHelperCreator: activator,
					defaultTagHelperActivator:
					new DefaultTagHelperActivator(provider.GetRequiredService<ITypeActivatorCache>())));
		}

		public static Type[] GetControllerTypes(this IApplicationBuilder builder)
		{
			var manager = builder.ApplicationServices.GetRequiredService<ApplicationPartManager>();

			var feature = new ControllerFeature();
			manager.PopulateFeature(feature);

			return feature.Controllers.Select(t => t.AsType()).ToArray();
		}

		public static Type[] GetViewComponentTypes(this IApplicationBuilder builder)
		{
			var manager = builder.ApplicationServices.GetRequiredService<ApplicationPartManager>();

			var feature = new ViewComponentFeature();
			manager.PopulateFeature(feature);

			return feature.ViewComponents.Select(t => t.AsType()).ToArray();
		}
	}
}