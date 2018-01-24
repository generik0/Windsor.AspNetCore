using System.Linq;
using Castle.Core;
using Castle.MicroKernel;
using Castle.MicroKernel.Context;
using Microsoft.Extensions.DependencyInjection;

namespace Castle.Facilities.AspNetCore
{
	public class FrameworkConfigurationDependencyResolver : ISubDependencyResolver
	{
		private readonly IServiceCollection serviceCollection;
		private readonly ServiceProvider serviceProvider;

		public FrameworkConfigurationDependencyResolver(IServiceCollection serviceCollection)
		{
			this.serviceCollection = serviceCollection;
			serviceProvider = serviceCollection.BuildServiceProvider();
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