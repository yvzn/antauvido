using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.IO;
using System;
using System.Text;

namespace Ludeo.Antauvido.Api
{
	public static class CreateDocument
	{
		[FunctionName("CreateDocument")]
		public static async Task RunAsync(
			[HttpTrigger(AuthorizationLevel.Function, "post", Route = "document")]
			HttpRequest request,
			IBinder binder, // use IBinder instead of blob output binding because path is dynamic
			ILogger logger)
		{
			var document = await ReadRequestAsync(request);

			var (documentId, blobPath) = await SaveDocumentAsync(document, binder);

			logger.LogInformation("Created document {BlobPath}", blobPath);

			if (request.HasFormContentType)
			{
				RedirectToPreview(request, documentId);
			}
			else
			{
				// no input provided, just create the document and return its URI
				await OutputDocumentLocationAsync(request, documentId);
			}
		}

		private static async Task<string> ReadRequestAsync(HttpRequest request)
		{
			if (request.HasFormContentType)
			{
				// TODO sanitize user input

				var form = await request.ReadFormAsync();
				var document = form["editor"];

				if (document.Count > 0)
				{
					return document[0];
				}
			}

			return "";
		}

		private static async Task<(Guid documentId, string blobPath)> SaveDocumentAsync(
			string document,
			IBinder binder)
		{
			var documentId = Guid.NewGuid();

			var blobPath = $"documents/{documentId}";
			using var outputStream = await binder.BindAsync<Stream>(
				new BlobAttribute(blobPath, FileAccess.Write));

			using var documentStream = new StreamWriter(outputStream);
			await documentStream.WriteAsync(document);

			return (documentId, blobPath);
		}

		private static void RedirectToPreview(HttpRequest request, Guid documentId)
		{
			var previewUri = $"/api/preview/{documentId}";
			request.HttpContext.Response.Headers.Add("Location", previewUri);

			request.HttpContext.Response.StatusCode = StatusCodes.Status303SeeOther;
		}

		private static async Task OutputDocumentLocationAsync(HttpRequest request, Guid documentId)
		{
			var documentUri = $"/api/document/{documentId}";
			request.HttpContext.Response.Headers.Add("Content-Location", documentUri);

			request.HttpContext.Response.StatusCode = StatusCodes.Status201Created;

			// if Content-Location header cannot be read (e.g. because of CORS) document URI is made available in response body
			await request.HttpContext.Response.Body.WriteAsync(Encoding.UTF8.GetBytes(documentUri));
		}
	}
}
