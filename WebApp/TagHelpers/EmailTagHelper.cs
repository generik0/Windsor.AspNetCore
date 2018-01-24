using Microsoft.AspNetCore.Razor.TagHelpers;

namespace WebApp.TagHelpers
{
	public class EmailTagHelper : TagHelper
	{
		private readonly IUserService userService;
		private const string EmailDomain = "contoso.com";
		public string MailTo { get; set; }

		public EmailTagHelper(IUserService userService)
		{
			this.userService = userService;
		}

		public override void Process(TagHelperContext context, TagHelperOutput output)
		{
			output.TagName = "a";    // Replaces <email> with <a> tag
			var address = MailTo + "@" + EmailDomain;
			output.Attributes.SetAttribute("href", "mailto:" + address);
			output.Content.SetContent(address);
		}
	}
}