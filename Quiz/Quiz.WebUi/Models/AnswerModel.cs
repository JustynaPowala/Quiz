using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
