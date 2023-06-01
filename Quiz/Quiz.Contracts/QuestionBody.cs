namespace Quiz.Contracts    
{
    public class QuestionBody
    {
        public string QuestionContent { get; set; }
        public int Points { get; set; }
        public string Category { get; set; }
        public AnswerMultiplicity SelectionMultiplicity { get; set; }
    }
}
