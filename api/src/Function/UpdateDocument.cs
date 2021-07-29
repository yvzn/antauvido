using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Storage.Blob;
using Ludeo.Antauvido.Api.Service;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;

namespace Ludeo.Antauvido.Api.Function
{
	public static class UpdateDocument
	{
		private static readonly DocumentRequestService documentRequestService = new DocumentRequestService();

		[FunctionName("UpdateDocument")]
		public static async Task<IActionResult> RunAsync(
			[HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "document/{documentId:guid}")]
			HttpRequest request,
			[Blob("documents")]
			CloudBlobContainer cloudBlobContainer,
			Guid documentId,
			ILogger logger)
		{
			var cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(documentId.ToString());
			if (!await cloudBlockBlob.ExistsAsync())
			{
				logger.LogInformation("Failed to update document {BlobPath} (not found)", $"documents/{documentId}");

				return new NotFoundResult();
			}

			var document = await documentRequestService.ReadDocumentAsync(request);

			await SaveDocumentAsync(document, cloudBlockBlob);

			logger.LogInformation("Updated document {BlobPath} with {BlobSize} chars", cloudBlockBlob.Name, document.Length);

			documentRequestService.RedirectToPreview(request, documentId);
			return new EmptyResult();
		}

		private static async Task SaveDocumentAsync(string document, CloudBlockBlob cloudBlockBlob)
		{
			using var outputStream = await cloudBlockBlob.OpenWriteAsync();

			using var documentStream = new StreamWriter(outputStream);
			await documentStream.WriteAsync(document);
		}
	}
}
