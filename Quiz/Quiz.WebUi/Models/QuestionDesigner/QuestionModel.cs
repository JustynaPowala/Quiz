using Quiz.Contracts;
using System.ComponentModel.DataAnnotations;

namespace Quiz.WebUi.Models.QuestionDesigner
{
    public class QuestionModel
    {
        public QuestionModel() { }
        public Guid ID { get; set; }

        [Required]
        public string? Question { get; set; }

        [Required]
        public int Points { get; set; }

        [Required]
        public string? Category { get; set; }

        [Required]
        public AnswerMultiplicity SelectionMultiplicity { get; set; }
        public QuestionActivityStatus ActivityStatus { get; set; }
    }
}
