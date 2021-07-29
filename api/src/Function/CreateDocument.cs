using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.IO;
using System;
using System.Text;
using Ludeo.Antauvido.Api.Service;
using Microsoft.AspNetCore.Mvc;

namespace Ludeo.Antauvido.Api.Function
{
	public static class CreateDocument
	{
		private static readonly DocumentRequestService documentRequestService = new DocumentRequestService();

		[FunctionName("CreateDocument")]
		public static async Task<IActionResult> RunAsync(
			[HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "document")]
			HttpRequest request,
			IBinder binder, // use IBinder instead of blob output binding because path is dynamic
			ILogger logger)
		{
			var document = await documentRequestService.ReadDocumentAsync(request);

			var (documentId, blobPath) = await SaveDocumentAsync(document, binder);

			logger.LogInformation("Created document {BlobPath} with {BlobSize} chars", blobPath, document.Length);

			if (request.HasFormContentType)
			{
				documentRequestService.RedirectToPreview(request, documentId);
			}
			else
			{
				// no input provided, just create the document and return its URI
				await ReplyRequestWithDocumentCreatedAsync(request, documentId);
			}

			return new EmptyResult();
		}

		private static async Task<(Guid documentId, string blobPath)> SaveDocumentAsync(
			string document,
			IBinder binder)
		{
			var documentId = Guid.NewGuid();

			var blobPath = $"documents/{documentId}";
			using var outputStream = await binder.BindAsync<Stream>(
				new BlobAttribute(blobPath, FileAccess.Write));

			using var documentWriter = new StreamWriter(outputStream);
			await documentWriter.WriteAsync(document);

			return (documentId, blobPath);
		}

		private static async Task ReplyRequestWithDocumentCreatedAsync(HttpRequest request, Guid documentId)
		{
			var documentUri = $"/api/document/{documentId}";
			request.HttpContext.Response.Headers.Add("Content-Location", documentUri);

			request.HttpContext.Response.StatusCode = StatusCodes.Status201Created;

			// if Content-Location header cannot be read (e.g. because of CORS) document URI is made available in response body
			await request.HttpContext.Response.Body.WriteAsync(Encoding.UTF8.GetBytes(documentUri));
			await request.HttpContext.Response.Body.FlushAsync();
		}
	}
}
