using Quiz.Contracts;

namespace Quiz.WebUi.ApiClients
{
    public interface IQuizApiClient
    {
        Task<Guid> AddQuestionAsync(string questionContent, int points, string category, AnswerMultiplicity selectionMultiplicity);

        Task<Guid> AddQuestionAnswerAsync(Guid questionID, string answerContent, bool isCorrect);

        Task DeleteAnswer(Guid questionID, Guid answerID);   
        Task DeleteQuestion(Guid questionID);

        Task<List<AnswerInfo>> GetListOfAnswersAsync(Guid questionID);
        Task<List<QuestionInfo>> GetQuestionsAsync(string category, int skipCount, int maxResultCount, string? searchString);

        Task<List<CategoryInfo>> GetCategoriesAsync();  //for both
        Task ModifyQuestionAsync(Guid questionID, string questionContent, int points, string category, AnswerMultiplicity selectionMultiplicity);
        Task<QuestionInfo> GetQuestionInfo(Guid questionID);

        Task ModifyAnswerAsync(Guid questionID, Guid answerID, string answerContent, bool isCorrect);

        Task ModifyQuestionStatusToActiveAsync(Guid questionID);

        Task <Guid> CloneQuestionAsync(Guid questionID);

        // Test methods below:
        Task<Guid> CreateTestAsync(List<string> listOfCategoriesIds);

        Task<TestQuestionBody> GetTestQuestionAsync(Guid testID, int skipCount);

        Task<int> GetQuestionsCountAsync(Guid testID);

        Task StartTestAsync(Guid testID);

        Task<List<TestQuestionAnswerBody>> GetListOfQuestionAnswers(Guid testID, Guid questionID);

        Task<Guid> AddAnswerToTestAnswers(Guid testID, Guid testQuestionID, Guid testAnswerID);
    }
}
