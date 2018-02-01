using System;
using System.Collections.Generic;
using System.Diagnostics;
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

			var baseAddress = "http://localhost:56163/";

			tasks.Add(Task.Factory.StartNew(() =>
			{
				var currentRequestsPerSecond = 0;
				var previousRequestsPerSecond = 0;
				var previousNumberOfSeconds = 0;
				var stopWatch = Stopwatch.StartNew();
				previousNumberOfSeconds = stopWatch.Elapsed.Seconds;
				while (true)
				{
					var currentNumberOfSeconds = stopWatch.Elapsed.Seconds;
					if (currentNumberOfSeconds > previousNumberOfSeconds)
					{
						previousNumberOfSeconds = currentNumberOfSeconds;
						previousRequestsPerSecond = currentRequestsPerSecond;
						currentRequestsPerSecond = 0;
					}
					TestMvc(client, baseAddress);
					currentRequestsPerSecond++;
					Console.WriteLine($"{previousRequestsPerSecond} requests/second");
				}
			}));

			Task.WaitAll(tasks.ToArray());
		}

		private static void TestMvc(HttpClient client, string baseAddress)
		{
			try
			{
				var response = client.GetAsync(baseAddress).GetAwaiter().GetResult();
				//Console.WriteLine(baseAddress + "::" + response.Content.ReadAsStringAsync().GetAwaiter().GetResult());
			}
			catch (Exception err)
			{
				Console.WriteLine(err.ToString());
			}
		}
	}
}