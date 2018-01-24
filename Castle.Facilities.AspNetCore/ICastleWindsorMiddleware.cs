using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Castle.Facilities.AspNetCore
{
	public interface ICastleWindsorMiddleware
	{
		Task Invoke(HttpContext context, Func<Task> next);
	}
}