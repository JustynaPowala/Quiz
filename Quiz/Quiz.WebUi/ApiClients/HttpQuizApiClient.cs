using Quiz.Contracts;
using System.Net.Http.Json;

namespace Quiz.WebUi.ApiClients

{
    public class HttpQuizApiClient : IQuizApiClient
    {
        private readonly IConfiguration _configuration;
        public HttpQuizApiClient(IConfiguration configuration) 
        { 
            _configuration = configuration;
        }
       
        public async Task<Guid> AddQuestionAsync(string questionContent, int points)
        {
            var client = CreateHttpClient();
            var response = await client.PostAsync("tests/to-database", JsonContent.Create(new AddQuestionBody()
            {
                QuestionContent = questionContent,
                Points = points
            }));

            var body = await response.Content.ReadFromJsonAsync<Guid>();
            return body;

        }

        private HttpClient CreateHttpClient()
        {
            var client= new HttpClient();
            client.BaseAddress = new Uri(_configuration["HttpClients:Quiz"]);    //from appsettings file (domyslnie)
            return client;
        }
    }
}








//dependency injection