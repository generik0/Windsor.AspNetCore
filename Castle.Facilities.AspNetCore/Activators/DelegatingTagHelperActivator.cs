using System;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Castle.Facilities.AspNetCore.Activators
{
	internal sealed class DelegatingTagHelperActivator : ITagHelperActivator
	{
		private readonly Predicate<Type> customCreatorSelector;
		private readonly Func<Type, object> customTagHelperCreator;
		private readonly ITagHelperActivator defaultTagHelperActivator;

		public DelegatingTagHelperActivator(Predicate<Type> customCreatorSelector, Func<Type, object> customTagHelperCreator,
			ITagHelperActivator defaultTagHelperActivator)
		{
			this.customCreatorSelector = customCreatorSelector ?? throw new ArgumentNullException(nameof(customCreatorSelector));
			this.customTagHelperCreator = customTagHelperCreator ?? throw new ArgumentNullException(nameof(customTagHelperCreator));
			this.defaultTagHelperActivator = defaultTagHelperActivator ?? throw new ArgumentNullException(nameof(defaultTagHelperActivator));
		}

		public TTagHelper Create<TTagHelper>(ViewContext context) where TTagHelper : ITagHelper =>
			this.customCreatorSelector(typeof(TTagHelper))
				? (TTagHelper)this.customTagHelperCreator(typeof(TTagHelper))
				: defaultTagHelperActivator.Create<TTagHelper>(context);
	}
}