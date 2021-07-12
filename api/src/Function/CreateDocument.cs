using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.IO;
using System;

namespace Ludeo.Antauvido.Api
{
	public static class CreateDocument
	{
		[FunctionName("CreateDocument")]
		public static async Task<IActionResult> RunAsync(
			[HttpTrigger(AuthorizationLevel.Function, "post", Route = "document")]
			HttpRequest request,
			IBinder binder,
			ILogger logger)
		{
			var documentId = Guid.NewGuid();

			// use IBinder from WebJobs because path of blob is dynamic
			var blobPath = $"documents/{documentId}";
			using var outputStream = await binder.BindAsync<Stream>(
				new BlobAttribute(blobPath, FileAccess.Write));

			await request.Body.CopyToAsync(outputStream);

			logger.LogInformation("Created document {BlobPath}", blobPath);

			request.HttpContext.Response.Headers.Add("Location", $"/api/preview/{documentId}");
			return new StatusCodeResult(StatusCodes.Status201Created);
		}
	}
}
