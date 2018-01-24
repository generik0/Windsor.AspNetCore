using System;
using System.Reflection;
using Castle.Facilities.AspNetCore.Activators;
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

			applicationTypeSelector = applicationTypeSelector ?? (type => !type.GetTypeInfo().Namespace.StartsWith("Microsoft"));

			services.AddSingleton<ITagHelperActivator>(provider =>
				new DelegatingTagHelperActivator(
					applicationTypeSelector,
					activator,
					new DefaultTagHelperActivator(provider.GetRequiredService<ITypeActivatorCache>())));
		}
	}
}