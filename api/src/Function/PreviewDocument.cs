using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using System.IO;
using Markdig;
using System.Text;
using Ganss.XSS;

namespace Ludeo.Antauvido.Api.Function
{
	public static class PreviewDocument
	{
		private static readonly MarkdownPipeline markdownPipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();
		private static readonly IHtmlSanitizer htmlSanitizer = new HtmlSanitizer();

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

			var markdown = await streamReader.ReadToEndAsync();

			var html = Markdown.ToHtml(markdown, markdownPipeline);
			var sanitized = htmlSanitizer.Sanitize(html);

			var result = $@"
<!DOCTYPE html>
<html>
<head>
	<title>Antaŭvido: document preview</title>
	<meta charset='UTF-8' />
	<meta name='viewport' content='width=device-width, initial-scale=1' />
</head>
<body>
	<main>{sanitized}</main>
</body>
</html>
";

			request.HttpContext.Response.StatusCode = StatusCodes.Status200OK;
			request.HttpContext.Response.Headers.Add("Content-Type", @"text/html; charset=""UTF-8""");

			await request.HttpContext.Response.Body.WriteAsync(Encoding.UTF8.GetBytes(result));
			await request.HttpContext.Response.Body.FlushAsync();
			return new EmptyResult();
		}
	}
}
