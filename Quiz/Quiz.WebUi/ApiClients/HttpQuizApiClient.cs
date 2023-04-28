using Quiz.Contracts;
using System.Net.Http;
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
       
        public async Task<Guid> AddQuestionAsync(string questionContent, int points, string category, string selectionMultiplicity)
        {
            var client = CreateHttpClient();
            var response = await client.PostAsync("tests/questions", JsonContent.Create(new AddQuestionBody()  // 
            {
                QuestionContent = questionContent,
                Points = points,
                Category = category,
                SelectionMultiplicity = selectionMultiplicity
            }));;

            var body = await response.Content.ReadFromJsonAsync<Guid>();
            return body;

        }

        public async Task<Guid> AddQuestionAnswerAsync(Guid questionID, string answerContent, bool isCorrect)
        {
            
            var client = CreateHttpClient();
            var address = "tests/questions/" + questionID+ "/answers";
            var response = await client.PostAsync(address, JsonContent.Create(new AddAnswerBody()  // 
            {
                AnswerContent = answerContent,
                IsCorrect = isCorrect
            })); ;

            var body = await response.Content.ReadFromJsonAsync<Guid>();  //identyfikattor odpowiedzi
            return body;
        }

        public async Task DeleteAnswer(Guid questionID, Guid answerID)
        {
            var client = CreateHttpClient();
            var address = "tests/questions/" + questionID + "/answers/" + answerID;
            await client.DeleteAsync(address);  
        }

        public async Task DeleteQuestion(Guid questionID)
        {
            var client = CreateHttpClient();
            var address = "tests/questions/" + questionID;
            await client.DeleteAsync(address);
        }

        public async Task<List<GetAllInfosAboutAnswer>> GetListOfAnswersAsync(Guid questionID)
        {

            var client = CreateHttpClient();
            var address = "tests/questions/" + questionID + "/answers";
            var response = await client.GetAsync(address);
            var listOfAnswers = await response.Content.ReadFromJsonAsync<List<GetAllInfosAboutAnswer>>();
            return listOfAnswers;
        }

        public async Task<List<QuestionInfo>> GetQuestionsAsync(string category, int skipCount, int maxResultCount, string? searchString)
        {
            var cat = category.ToLower();  //tolower is to delete
            var searchStr = searchString.ToLower();
            var skippedCount = skipCount;
            var maxResult = maxResultCount;    //ints to strings?



            var client = CreateHttpClient();
            var address = "tests/questions?category=" + cat + "&skipCount=" + skippedCount + "&maxResultCount=" + maxResult;
            if(searchStr != null)
            {
                address += "&searchString=" + searchStr;
            }
            
            var response = await client.GetAsync(address);
            var listOfQuestions = await response.Content.ReadFromJsonAsync<List<QuestionInfo>>();
            return listOfQuestions;
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