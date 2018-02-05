﻿// Copyright 2004-2018 Castle Project - http://www.castleproject.org/
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

using Microsoft.AspNetCore.Http;

namespace Castle.Facilities.AspNetCore
{
	using System;
	using System.Linq;

	using Castle.Facilities.AspNetCore.Resolvers;
	using Castle.MicroKernel.Lifestyle;
	using Castle.MicroKernel.Registration;
	using Castle.Windsor;

	using Microsoft.AspNetCore.Builder;
	using Microsoft.AspNetCore.Mvc;
	using Microsoft.AspNetCore.Razor.TagHelpers;
	using Microsoft.Extensions.DependencyInjection;

	public static class CastleWindsorRegistrationExtensions
	{
		/// <summary>
		/// Sets up framework level activators for Controllers, TagHelpers and ViewComponents and adds additional sub dependency resolvers
		/// </summary>
		/// <param name="services">ASP.NET Core service collection from Microsoft.Extensions.DependencyInjection</param>
		/// <param name="container">Windsor container which activators call resolve against</param>
		public static void AddCastleWindsor(this IServiceCollection services, IWindsorContainer container)
		{
			services.AddRequestScopingMiddleware(container.BeginScope);
			services.AddCustomControllerActivation(container.Resolve);
			services.AddCustomTagHelperActivation(container.Resolve);
			services.AddCustomViewComponentActivation(container.Resolve);
			container.Kernel.Resolver.AddSubResolver(new LoggerDependencyResolver(services));
			container.Kernel.Resolver.AddSubResolver(new FrameworkConfigurationDependencyResolver(services));
		}

		/// <summary>
		/// Use this to register all Controllers, ViewComponents and TagHelpers
		/// </summary>
		/// <typeparam name="TTypeAssembly">Type from assembly to scan and register ASP.NET Core components</typeparam>
		/// <param name="app">Application builder retained as extension</param>
		/// <param name="container">Windsor container to register framework types in</param>
		public static void UseCastleWindsor<TTypeAssembly>(this IApplicationBuilder app, IWindsorContainer container)
		{
			container.Register(Classes.FromAssemblyInThisApplication(typeof(TTypeAssembly).Assembly).BasedOn<Controller>().LifestyleScoped());
			container.Register(Classes.FromAssemblyInThisApplication(typeof(TTypeAssembly).Assembly).BasedOn<ViewComponent>().LifestyleTransient());
			container.Register(Classes.FromAssemblyInThisApplication(typeof(TTypeAssembly).Assembly).BasedOn<TagHelper>().LifestyleTransient());
		}

		/// <summary>
		/// For registering middleware that consumes services in the constructor known to Castle Windsor. You can use 
		/// conventional methods for registering your middleware but then you have to re-register your dependencies
		/// in the ASP.NET IServiceCollection. You should try and avoid that where ever possible.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="app"></param>
		/// <param name="container"></param>
		public static void UseResolvableMiddleware<T>(this IApplicationBuilder app, IWindsorContainer container) where T : class, IMiddleware
		{
			container.Register(Component.For<T>());
			app.Use(async (context, next) =>
			{
				var resolve = container.Resolve<T>();
				await resolve.InvokeAsync(context, async (ctx) => await next());
			});
		}

		/// <summary>
		/// Optional helpful exception to validate against types being installed into Windsor from the 'Microsoft.AspNetCore'
		/// namespace. This guards against 'Torn Lifestyles' and 'Captive Depenencies'.
		/// </summary>
		/// <param name="container">Windsor container</param>
		public static void ValidateConfiguration(this IWindsorContainer container)
		{
			ThrowIfFrameworkComponentsAreRegistered(container);
		}

		private static void ThrowIfFrameworkComponentsAreRegistered(IWindsorContainer container)
		{
			var handlers = container.Kernel.GetHandlers();
			var hasAspNetCoreFrameworkRegistrations = handlers.Any(x => x.ComponentModel.Implementation.Namespace.StartsWith("Microsoft.AspNetCore"));
			if (hasAspNetCoreFrameworkRegistrations)
			{
				throw new Exception(
					"Looks like you have implementations registered from 'Microsoft.AspNetCore'. " +
					"Please do not do this as it could lead to torn lifestyles and captive dependencies. " +
					"Please remove the registrations from Castle.Windsor.");
			}
		}
	}
}