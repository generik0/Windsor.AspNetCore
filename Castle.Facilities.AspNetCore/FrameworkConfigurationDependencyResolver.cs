// Copyright 2004-2018 Castle Project - http://www.castleproject.org/
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Runtime.CompilerServices;

namespace Castle.Facilities.AspNetCore
{
	using System.Linq;

	using Castle.Core;
	using Castle.MicroKernel;
	using Castle.MicroKernel.Context;

	using Microsoft.Extensions.DependencyInjection;

	public class FrameworkConfigurationDependencyResolver : ISubDependencyResolver
	{
		private readonly ServiceProvider serviceProvider;
		private readonly IServiceCollection serviceCollection;

		public FrameworkConfigurationDependencyResolver(IServiceCollection serviceCollection)
		{
			this.serviceCollection = serviceCollection;
			serviceProvider = serviceCollection.BuildServiceProvider(); // This might be to early, Will add a note to the docs.
		}

		public bool CanResolve(CreationContext context, ISubDependencyResolver contextHandlerResolver, ComponentModel model, DependencyModel dependency)
		{
			var dependencyType = dependency.TargetType;

			// Checking if the dependency type is a generic
			if (dependencyType.IsGenericType)
			{
				// TODO: Generics need further supported definition. Code below should be able to delegate most of the permutations. Performance might also be a consideration here.

				// Demanded service type, unwrapped
				var dependencyTypeGenericTypeDefinition = dependencyType.GetGenericTypeDefinition();

				// Generics mixed
				var genericServiceTypes = serviceCollection.Where(x => x.ServiceType.IsGenericType).ToList();

				// Generics exhaustively matching generic type arguments and counts. No palindrome validation.
				var servicesWithGenericTypeDefinitionsAndParameters = genericServiceTypes.Where(x => x.ServiceType.GenericTypeArguments.Length > 0).ToList();
				var matchingGenericTypeDefinitions = servicesWithGenericTypeDefinitionsAndParameters.Any(x => x.ServiceType.GetGenericTypeDefinition() == dependencyTypeGenericTypeDefinition && x.ServiceType.GenericTypeArguments.All(sy => dependencyType.GetGenericArguments().Contains(sy)));
				if (matchingGenericTypeDefinitions)
					return matchingGenericTypeDefinitions;

				// TODO: Edge cases here

				// Open generic services with open generic implementations. Might need refinement.
				var servicesWithGenericTypeDefinitionsAndNoParameters = genericServiceTypes.Where(x => x.ServiceType.GenericTypeArguments.Length == 0 && x.ImplementationType.GenericTypeArguments.Length == 0).ToList();
				matchingGenericTypeDefinitions = servicesWithGenericTypeDefinitionsAndNoParameters.Any(x => x.ServiceType == dependencyTypeGenericTypeDefinition);
				if (matchingGenericTypeDefinitions)
					return matchingGenericTypeDefinitions;

				return false;
			}

			// Going for 1..N scan on all types
			var canResolveWithoutGenerics = serviceCollection.Any(x => x.ServiceType == dependencyType);
			if (canResolveWithoutGenerics)
				return canResolveWithoutGenerics;

			return false;
		}

		public object Resolve(CreationContext context, ISubDependencyResolver contextHandlerResolver, ComponentModel model, DependencyModel dependency)
		{
			return serviceProvider.GetService(dependency.TargetType);
		}
	}
}