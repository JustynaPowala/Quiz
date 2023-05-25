using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quiz.Contracts
{
    public class TestQuestionAnswerBody
    {

        public TestQuestionAnswerBody()
        {
           
            AnswerContent = String.Empty;
            //IsCorrect = false;
            AnswGuid = Guid.Empty;
            IsSelected = false;

        }
        public TestQuestionAnswerBody(string answerContent, Guid answGuid, bool isSelected)  
        {
            AnswerContent = answerContent;
            //IsCorrect = isCorrect;
            AnswGuid = answGuid;
            IsSelected = isSelected;
        }


        public string AnswerContent { get; set; }
        //public bool IsCorrect { get; set; }
        public Guid AnswGuid { get; set; }

        public bool IsSelected { get; set; }
    }



}

