using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace Ludeo.Antauvido.Api.Function
{
	public static class PreviewDocument
	{
		[FunctionName("PreviewDocument")]
		public static async Task<IActionResult> RunAsync(
			[HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "preview/{documentId:guid}")]
			HttpRequest request,
			[Blob("documents/{documentId}", FileAccess.Read)]
			Stream? inputStream)
		{
			if (inputStream is null)
			{
				return new NotFoundResult();
			}

			using var streamReader = new StreamReader(inputStream);

			// TODO parse and render Markdown document
			var content = await streamReader.ReadToEndAsync();

			return new OkObjectResult(content);
		}
	}
}
