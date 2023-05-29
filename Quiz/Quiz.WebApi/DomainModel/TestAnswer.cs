namespace Quiz.WebApi.DomainModel
{
    public class TestAnswer
    {
        public Guid AnswerId { get; set; }
        public bool IsCorrect { get; set; }
        public bool IsSelected { get; set; }
    }
}
