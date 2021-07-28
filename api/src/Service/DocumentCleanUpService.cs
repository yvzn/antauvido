using System;
using System.Threading.Tasks;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Extensions.Logging;

namespace Ludeo.Antauvido.Api.Service
{
	internal class DocumentCleanUpService
	{
		public async Task CleanUpDocumentsAsync(
			CloudBlobContainer cloudBlobContainer,
			Predicate<CloudBlockBlob> isDocumentToBeDeleted,
			ILogger logger)
		{
			BlobContinuationToken? continuationToken = default;

			do
			{
				var blobs = await cloudBlobContainer.ListBlobsSegmentedAsync(continuationToken);
				continuationToken = blobs.ContinuationToken;

				foreach (var blob in blobs.Results)
				{
					if (blob is CloudBlockBlob document)
					{
						await document.FetchAttributesAsync();
						logger.LogInformation("Blob {BlobPath} has ~{BlobSize} chars and last modified ~{BlobLastModifiedOffset:0.00} days ago", document.Name, document.Properties.Length, (DateTimeOffset.Now - document.Properties.LastModified)?.TotalDays);

						if (isDocumentToBeDeleted.Invoke(document))
						{
							await document.DeleteIfExistsAsync();
							logger.LogInformation("Deleted blob {BlobPath}", document.Name);
						}
					}
				}
			} while (continuationToken != null);
		}
	}
}
