using System.ComponentModel.DataAnnotations;

namespace Quiz.WebUi.Models
{
    public class AnswerModel
    {
        public AnswerModel() { }
        
            [Required]
            public string? Answer { get; set; }
            public bool IsCorrect { get; set; }

        
    }
}
