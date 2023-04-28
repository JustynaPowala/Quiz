using System.ComponentModel.DataAnnotations;

namespace Quiz.WebUi.Classes
{
    public class QuestionModel
    {
        public QuestionModel() { }


            [Required]
            public string? Question { get; set; }

            [Required]
            public int Points { get; set; }

            [Required]
            public string? Category { get; set; }

            [Required]
            public string? SelectionMultiplicity { get; set; }
        
        

    }
}
