using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Ludeo.Antauvido.Api.Service
{
	public class DocumentRequestService
	{
		internal async Task<string> ReadDocumentAsync(HttpRequest request)
		{
			if (request.HasFormContentType)
			{
				// TODO sanitize user input

				var form = await request.ReadFormAsync();
				var document = form["document"];

				if (document.Count > 0)
				{
					return document[0] ?? "";
				}
			}

			return "";
		}

		internal void RedirectToPreview(HttpRequest request, Guid documentId)
		{
			var previewUri = $"/api/preview/{documentId}";
			request.HttpContext.Response.Headers.Add("Location", previewUri);

			request.HttpContext.Response.StatusCode = StatusCodes.Status303SeeOther;
		}
	}
}
