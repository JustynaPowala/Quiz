namespace Quiz.Contracts    // wspólna bibliotka dla projektów web api i web ui
{
    public class AddQuestionBody
    {
        public string QuestionContent { get; set; }
        public int Points { get; set; }

        public string Category { get; set; }

        public AnswerMultiplicity SelectionMultiplicity { get; set; }
    }
}
