using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quiz.Contracts
{
    public class TestQuestionBody
    {

        public TestQuestionBody()
        {
            QuestionContent = String.Empty;
            Points = 0;
            AnswerMultiplicity = AnswerMultiplicity.Single;
            QGuid = Guid.Empty;
            //AnswerContent = String.Empty;
            //IsCorrect = false;
            //AnswGuid = Guid.Empty;

        }
        public TestQuestionBody(string questionContent, int points, AnswerMultiplicity answerMultiplicity, Guid qGuid)  
        {
            QuestionContent = questionContent;            
            Points = points;
            AnswerMultiplicity = answerMultiplicity;
            QGuid= qGuid;
            //AnswerContent = answerContent;
            //IsCorrect = isCorrect;
            //AnswGuid = answGuid;
        }

        public string QuestionContent { get; set; }
        public int Points { get; set; }
        public AnswerMultiplicity AnswerMultiplicity { get; set; }  
        public Guid QGuid { get; set; }
        //public string AnswerContent { get; set; }
        //public bool IsCorrect { get; set; }
        //public Guid AnswGuid { get; set; }
    }



}


//, string answerContent, bool isCorrect, Guid answGuid