using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace PerfApp
{
	internal class Program
	{
		private static void Main(string[] args)
		{
			var client = new HttpClient();

			var tasks = new List<Task>();

			var baseAddress = "http://localhost:56162/";

			tasks.Add(Task.Factory.StartNew(() =>
			{
				while (true) TestMvc(client, baseAddress);
			}));

			Task.WaitAll(tasks.ToArray());
		}

		private static void TestMvc(HttpClient client, string baseAddress)
		{
			try
			{
				var response = client.GetAsync(baseAddress).GetAwaiter().GetResult();
				Console.WriteLine(baseAddress + "::" + response.Content.ReadAsStringAsync().GetAwaiter().GetResult());
			}
			catch (Exception err)
			{
				Console.WriteLine(err.ToString());
			}
		}
	}
}