﻿using Quiz.Contracts;
using Quiz.WebUi.Pages;
using System.Net.Http;
using System.Net.Http.Json;
using System.Net;

namespace Quiz.WebUi.ApiClients
{
    public class HttpQuizApiClient : IQuizApiClient
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public HttpQuizApiClient(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<Guid> AddQuestionAsync(string questionContent, int points, string category, AnswerMultiplicity selectionMultiplicity)
        {
            var client = CreateHttpClient();
            var response = await client.PostAsync("questions", JsonContent.Create(new QuestionBody()   
            {
                QuestionContent = questionContent,
                Points = points,
                Category = category,
                SelectionMultiplicity = selectionMultiplicity
            })); ;  
                var body = await response.Content.ReadFromJsonAsync<Guid>();
                return body;                     
        }

        public async Task<Guid> AddQuestionAnswerAsync(Guid questionID, string answerContent, bool isCorrect)
        {
            var client = CreateHttpClient();
            var address = "questions/" + questionID + "/answers";
            var response = await client.PostAsync(address, JsonContent.Create(new AnswerBody()  
            {
                AnswerContent = answerContent,
                IsCorrect = isCorrect
            })); ;

            var body = await response.Content.ReadFromJsonAsync<Guid>(); 
            return body;
        }

        public async Task DeleteAnswer(Guid questionID, Guid answerID)
        {
            var client = CreateHttpClient();
            var address = "questions/" + questionID + "/answers/" + answerID;
            await client.DeleteAsync(address);
        }

        public async Task DeleteQuestion(Guid questionID)
        {
            var client = CreateHttpClient();
            var address = "questions/" + questionID;
            await client.DeleteAsync(address);
        }

        public async Task<List<AnswerInfo>> GetListOfAnswersAsync(Guid questionID)
        {
            var client = CreateHttpClient();
            var address = "questions/" + questionID + "/answers";
            var response = await client.GetAsync(address);
            var listOfAnswers = await response.Content.ReadFromJsonAsync<List<AnswerInfo>>();
            return listOfAnswers;
        }

        public async Task<List<QuestionInfo>> GetQuestionsAsync(string category, int skipCount, int maxResultCount, string? searchString)
        {
            var cat = category;
            var searchStr = searchString;
            var skippedCount = skipCount;
            var maxResult = maxResultCount;

            var client = CreateHttpClient();
            var address = "questions?category=" + cat + "&skipCount=" + skippedCount + "&maxResultCount=" + maxResult;
            if (searchStr != null)
            {
                address += "&searchString=" + searchStr;
            }
            var response = await client.GetAsync(address);
            var listOfQuestions = await response.Content.ReadFromJsonAsync<List<QuestionInfo>>();
            return listOfQuestions;
        }

        public async Task<List<CategoryInfo>> GetCategoriesAsync()
        {
            var client = CreateHttpClient();
            var address = "categories";
            var response = await client.GetAsync(address);
            var listOfCategories = await response.Content.ReadFromJsonAsync<List<CategoryInfo>>();
            return listOfCategories;
        }


        public async Task ModifyQuestionAsync(Guid questionID, string questionContent, int points, string category, AnswerMultiplicity selectionMultiplicity)
        {
            var client = CreateHttpClient();
            var address = "questions/" + questionID;
            await client.PutAsync(address, JsonContent.Create(new QuestionBody()
            {
                QuestionContent = questionContent,
                Points = points,
                Category = category,
                SelectionMultiplicity = selectionMultiplicity
            }));
        }


        public async Task<QuestionInfo> GetQuestionInfo(Guid questionID)
        {
            var client = CreateHttpClient();
            var address = "questions/" + questionID;
            await client.GetAsync(address);
            var response = await client.GetAsync(address);
            var infoAboutQuestion = await response.Content.ReadFromJsonAsync<QuestionInfo>();
            return infoAboutQuestion;
        }

        public async Task ModifyAnswerAsync(Guid questionID, Guid answerID, string answerContent, bool isCorrect)
        {
            var client = CreateHttpClient();
            var address = "questions/" + questionID + "/answers/" + answerID;
            var response = await client.PutAsync(address, JsonContent.Create(new AnswerBody()
            {
                AnswerContent = answerContent,
                IsCorrect = isCorrect
            })); ;
        }

        public async Task ModifyQuestionStatusToActiveAsync(Guid questionID)
        {
            var client = CreateHttpClient();
            var address = "questions/" + questionID + "/activate";
            await client.PostAsync(address, null);
        }

        public async Task<Guid> CloneQuestionAsync(Guid questionID)
        {
            var client = CreateHttpClient();
            var address = "questions/" + questionID + "/clone";
            var response = await client.PostAsync(address, null);
            var body = await response.Content.ReadFromJsonAsync<Guid>();
            return body;
        }

        public async Task<Guid> CreateTestAsync(List<string> listOfCategoriesIds)
        {
            var client = CreateHttpClient();
            var response = await client.PostAsync("tests", JsonContent.Create(new CreateTestBody()
            {
                CategoriesIds = listOfCategoriesIds
            }));
            var body = await response.Content.ReadFromJsonAsync<Guid>();
            return body;
        }

        public async Task<TestQuestionBody> GetTestQuestionAsync(Guid testID, int skipCount)
        {
            var skippedCount = skipCount;
            var client = CreateHttpClient();
            var address = "tests/" + testID + "/questions/" + "?skipCount=" + skippedCount;
            var response = await client.GetAsync(address);
            var TestQA = await response.Content.ReadFromJsonAsync<TestQuestionBody>();
            return TestQA;
        }

        public async Task<int> GetQuestionsCountAsync(Guid testID)
        {
            var client = CreateHttpClient();
            var address = "tests/" + testID + "/questions/count";
            var response = await client.GetAsync(address);
            var stringCount = await response.Content.ReadAsStringAsync();
            var count = int.Parse(stringCount);
            return count;
        }

        public async Task StartTestAsync(Guid testID)
        {
            var client = CreateHttpClient();
            var address = "tests/" + testID + "/start";
            await client.PutAsync(address, null);
        }

        public async Task EndTestAsync(Guid testID)
        {
            var client = CreateHttpClient();
            var address = "tests/" + testID + "/end-test";
            await client.PutAsync(address, null);
        }

        public async Task<List<TestQuestionAnswerBody>> GetListOfQuestionAnswers(Guid testID, Guid questionID)
        {
            var client = CreateHttpClient();
            var address = "tests/" + testID + "/questions/" + questionID + "/answers";
            var response = await client.GetAsync(address);
            var listOfQuestionAnswers = await response.Content.ReadFromJsonAsync<List<TestQuestionAnswerBody>>();
            return listOfQuestionAnswers;
        }

        public async Task<Guid> AddAnswerToTestAnswers(Guid testID, Guid testQuestionID, Guid testAnswerID)
        {
            var client = CreateHttpClient();
            var address = "tests/" + testID + "/test-questions/" + testQuestionID + "/test-answers";
            var response = await client.PostAsync(address, JsonContent.Create(new AddAnswerToTestAnswersBody()
            {
                AnswGuid = testAnswerID
            }));
            var body = await response.Content.ReadFromJsonAsync<Guid>();
            return body;
        }

        public async Task DeleteAnswerFromTestAnswers(Guid testID, Guid testQuestionID, Guid answerID)
        {
            var client = CreateHttpClient();
            var address = "tests/" + testID + "/test-questions/" + testQuestionID + "/test-answers/" + answerID;
            var response = await client.DeleteAsync(address);
        }
        public async Task<TestResultBody> GetResultAsync(Guid testID)
        {
            var client = CreateHttpClient();
            var address = "tests/" + testID + "/result";
            var response = await client.GetAsync(address);
            var result = await response.Content.ReadFromJsonAsync<TestResultBody>();
            return result;
        }
        private HttpClient CreateHttpClient()
        {
            var client = _httpClientFactory.CreateClient("QuizHttpClient");  
            return client;
        }
    }
}








