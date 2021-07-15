using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;

namespace Ludeo.Antauvido.Api
{
	public static class HealthCheck
	{
		[FunctionName("HealthCheck")]
		public static IActionResult Run(
			[HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "healthz")]
			HttpRequest request)
		{
			return new OkObjectResult("Healthy");
		}
	}
}
