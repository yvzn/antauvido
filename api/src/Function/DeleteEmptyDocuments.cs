using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.Storage.Blob;
using Ludeo.Antauvido.Api.Service;

namespace Ludeo.Antauvido.Api.Function
{
	public static class DeleteEmptyDocuments
	{
		private static DocumentCleanUpService documentCleanUpService = new DocumentCleanUpService();

		[FunctionName("DeleteEmptyDocuments")]
		public static async Task RunAsync(
			[TimerTrigger("0 15 */12 * * *"
#if DEBUG
				, RunOnStartup=true
#endif
			)]
			TimerInfo timerInfo,
			[Blob("documents")]
			CloudBlobContainer cloudBlobContainer,
			ILogger logger)
		{
			var minimumDocumentSizeString = System.Environment.GetEnvironmentVariable("AntauvidoDocumentMinimumSize");
			if (!int.TryParse(minimumDocumentSizeString, out var minimumDocumentSize))
			{
				minimumDocumentSize = 1;
			}

			await documentCleanUpService.CleanUpDocumentsAsync(
				cloudBlobContainer,
				document => document.Properties.Length < minimumDocumentSize,
				logger);
		}
	}
}
