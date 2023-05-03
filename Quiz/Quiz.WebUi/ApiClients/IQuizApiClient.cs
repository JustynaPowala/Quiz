using Quiz.Contracts;

namespace Quiz.WebUi.ApiClients
{
    public interface IQuizApiClient
    {
        Task<Guid> AddQuestionAsync(string questionContent, int points, string category, AnswerMultiplicity selectionMultiplicity);

        Task<Guid> AddQuestionAnswerAsync(Guid questionID, string answerContent, bool isCorrect);

        Task DeleteAnswer(Guid questionID, Guid answerID);   //async?
        Task DeleteQuestion(Guid questionID);

        Task<List<GetAllInfosAboutAnswer>> GetListOfAnswersAsync(Guid questionID);
        Task<List<QuestionInfo>> GetQuestionsAsync(string category, int skipCount, int maxResultCount, string? searchString);

        Task<List<CategoryInfo>> GetCategoriesAsync();
        Task ModifyQuestionAsync(Guid questionID, string questionContent, int points, string category, AnswerMultiplicity selectionMultiplicity);
        Task<QuestionInfo> GetQuestionInfosToModifyAsync(Guid questionID);
    }
}
