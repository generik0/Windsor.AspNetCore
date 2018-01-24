using System;
using System.Linq;
using Castle.Core;
using Castle.MicroKernel;
using Castle.MicroKernel.Context;
using Microsoft.Extensions.DependencyInjection;

namespace Castle.Facilities.AspNetCore
{
	public class FrameworkConfigurationDependencyResolver : ISubDependencyResolver
	{
		private readonly ServiceProvider serviceProvider;
		private readonly IServiceCollection serviceCollection;

		public FrameworkConfigurationDependencyResolver(IServiceCollection serviceCollection)
		{
			this.serviceCollection = serviceCollection;
			this.serviceProvider = serviceCollection.BuildServiceProvider();
		}

		public bool CanResolve(CreationContext context, ISubDependencyResolver contextHandlerResolver, ComponentModel model, DependencyModel dependency)
		{
			var dependencyType = dependency.TargetType;
			return serviceCollection.Any(x => x.ServiceType == dependencyType);
		}

		public object Resolve(CreationContext context, ISubDependencyResolver contextHandlerResolver, ComponentModel model, DependencyModel dependency)
		{
			return serviceProvider.GetService(dependency.TargetType);
		}
	}
}