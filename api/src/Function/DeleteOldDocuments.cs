using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.Storage.Blob;
using Ludeo.Antauvido.Api.Service;
using System;

namespace Ludeo.Antauvido.Api.Function
{
	public static class DeleteOldDocuments
	{
		private static readonly DocumentCleanUpService documentCleanUpService = new DocumentCleanUpService();

		[FunctionName("DeleteOldDocuments")]
		public static async Task RunAsync(
			[TimerTrigger("0 15 13 * * *"
#if DEBUG
				, RunOnStartup=true
#endif
			)]
			TimerInfo timerInfo,
			[Blob("documents")]
			CloudBlobContainer cloudBlobContainer,
			ILogger logger)
		{
			var documentTimeoutString = System.Environment.GetEnvironmentVariable("AntauvidoDocumentTimeOut");
			if (!TimeSpan.TryParse(documentTimeoutString, out var documentTimeout))
			{
				documentTimeout = TimeSpan.FromDays(30);
			}

			await documentCleanUpService.CleanUpDocumentsAsync(
				cloudBlobContainer,
				document => !(document.Properties.LastModified.HasValue) || (DateTimeOffset.Now - document.Properties.LastModified > documentTimeout),
				logger);
		}
	}
}
