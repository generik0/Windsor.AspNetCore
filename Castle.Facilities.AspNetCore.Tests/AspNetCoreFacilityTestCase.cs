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
using Castle.MicroKernel.Lifestyle;
using Castle.MicroKernel.Registration;
using Microsoft.AspNetCore.Builder.Internal;

namespace Castle.Facilities.AspNetCore.Tests
{
	using NUnit.Framework;

	using Castle.Windsor;

	using Microsoft.Extensions.DependencyInjection;
	using Microsoft.AspNetCore.Mvc;
	using Microsoft.AspNetCore.Razor.TagHelpers;

	[TestFixture]
    public class AspNetCoreFacilityTestCase
    {
	    private IDisposable scope;
	    private WindsorContainer container;

	    [Test]
	    public void Should_be_able_to_resolve_controller_from_container()
	    {
		    Assert.DoesNotThrow(() => container.Resolve<AnyController>());
	    }

	    [Test]
	    public void Should_be_able_to_resolve_view_component_from_container()
	    {
		    Assert.DoesNotThrow(() => container.Resolve<AnyViewComponent>());
	    }

	    [Test]
	    public void Should_be_able_to_resolve_tag_helper_from_container()
	    {
		    Assert.DoesNotThrow(() => container.Resolve<AnyTagHelper>());
	    }

	    [OneTimeSetUp]
	    public void SetUp()
	    {
		    var serviceCollection = new ServiceCollection();
		    serviceCollection.AddTransient<IFakeFrameworkComponent, FakeFrameworkComponent>();

		    container = new WindsorContainer();
		    serviceCollection.AddCastleWindsor(container);
		    container.Register(Component.For<AnyService>());

		    var applicationBuilder = new ApplicationBuilder(serviceCollection.BuildServiceProvider());
		    applicationBuilder.UseCastleWindsor<AspNetCoreFacilityTestCase>(container);

		    scope = container.BeginScope();
	    }

	    [OneTimeTearDown]
	    public void TearDown()
	    {
		    scope.Dispose();
		    container.Dispose();
	    }

	    public class AnyService { }

	    public class AnyController : Controller
	    {
		    private readonly AnyService service;
		    private readonly IFakeFrameworkComponent component;

		    public AnyController(AnyService service, IFakeFrameworkComponent component)
		    {
			    this.service = service ?? throw new ArgumentNullException(nameof(service));
			    this.component = component ?? throw new ArgumentNullException(nameof(component));
		    }
	    }

	    public class AnyTagHelper : TagHelper
	    {
		    private readonly AnyService service;
		    private readonly IFakeFrameworkComponent component;

		    public AnyTagHelper(AnyService service, IFakeFrameworkComponent component)
		    {
			    this.service = service ?? throw new ArgumentNullException(nameof(service));
			    this.component = component ?? throw new ArgumentNullException(nameof(component));
		    }
	    }

	    public class AnyViewComponent : ViewComponent
	    {
		    private readonly AnyService service;
		    private readonly IFakeFrameworkComponent component;

		    public AnyViewComponent(AnyService service, IFakeFrameworkComponent component)
		    {
			    this.service = service ?? throw new ArgumentNullException(nameof(service));
			    this.component = component ?? throw new ArgumentNullException(nameof(component));
		    }
	    }

		public interface IFakeFrameworkComponent { }
	    public class FakeFrameworkComponent : IFakeFrameworkComponent
	    {
	    }
	}
}
