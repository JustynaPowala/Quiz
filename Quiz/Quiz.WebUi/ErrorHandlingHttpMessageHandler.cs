using Quiz.Contracts;
using Quiz.WebUi.ApiClients;
using System.Net.Http.Json;
using System.Reflection.Metadata.Ecma335;

namespace Quiz.WebApi
{
	public class ErrorHandlingHttpMessageHandler : DelegatingHandler
	{
		protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
		{
			var response = await base.SendAsync(request, cancellationToken);
			if (!response.IsSuccessStatusCode)
			{
				var errorInfo = await response.Content.ReadFromJsonAsync<ErrorInfo>();

				throw new QuizApiException()
				{
					Message = errorInfo.Message
				};
			}
			return response;
		}

	}
}