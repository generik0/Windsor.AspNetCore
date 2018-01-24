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

namespace Castle.Facilities.AspNetCore
{
	using Castle.MicroKernel.Lifestyle;
	using Castle.MicroKernel.Registration;
	using Castle.Windsor;

	using Microsoft.AspNetCore.Builder;
	using Microsoft.AspNetCore.Http;
	using Microsoft.AspNetCore.Mvc;
	using Microsoft.AspNetCore.Razor.TagHelpers;
	using Microsoft.Extensions.DependencyInjection;

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