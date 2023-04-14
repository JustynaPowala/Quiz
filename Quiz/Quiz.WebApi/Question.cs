namespace Quiz.WebApi
{
    public class Question
    {
        private string _contentOfQuestion;
        private int _numberOfAnswers;
        private int _numberOfCorrectAnswers;
        private int _correctAnswers;

        Question(string contentOfQuestion, int numberOfAnswers, int numberOfCorrectAnswers, int correctAnswers)
        {
            _contentOfQuestion = contentOfQuestion;
            _numberOfAnswers = numberOfAnswers;
            _numberOfCorrectAnswers = numberOfCorrectAnswers;
            _correctAnswers = correctAnswers;
        }   

        public string ContentOfQuestion => _contentOfQuestion;
        public int NumberOfAnswers => _numberOfAnswers;
        public int NumberOfCorrectAnswers => _numberOfCorrectAnswers;
        public int CorrectAnswers => _correctAnswers;

    }
}