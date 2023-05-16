using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quiz.Contracts
{
    public class AnswerBody
    {
        //public Guid QuestionID { get; set; }

        public string AnswerContent { get; set; }

        public bool IsCorrect { get; set; }


    }
}
