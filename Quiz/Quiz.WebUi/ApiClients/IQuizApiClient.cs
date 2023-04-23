using Quiz.Contracts;

namespace Quiz.WebUi.ApiClients
{
    public interface IQuizApiClient
    {
        Task<Guid> AddQuestionAsync(string questionContent, int points, string category, string selectionMultiplicity);

        Task<Guid> AddQuestionAnswerAsync(Guid questionID, string answerContent, bool isCorrect);

        Task DeleteAnswer(Guid questionID, Guid answerID);   //async?

        Task<List<GetAllInfosAboutAnswer>> GetListOfAnswersAsync(Guid questionID);
    }
}
