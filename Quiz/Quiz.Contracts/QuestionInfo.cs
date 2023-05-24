using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quiz.Contracts
{
    public class QuestionInfo
    {
       public QuestionInfo() 
        {
            Guid = Guid.Empty;
            QuestionContent = String.Empty; 
            Category = String.Empty;
            Points = 0;
            AnswerMultiplicity = AnswerMultiplicity.Single;
            ActivityStatus = QuestionActivityStatus.InPreparation;
            
        }
       public QuestionInfo(Guid guid, string questionContent, string category, int points, AnswerMultiplicity answerMultiplicity, QuestionActivityStatus activityStatus)  // 2 różne przeciążenia tej samej metody
        {
            Guid = guid;
            QuestionContent = questionContent;
            Category = category;
            Points = points;
            AnswerMultiplicity = answerMultiplicity;
            ActivityStatus = activityStatus;
            
        }
        public Guid Guid { get; set; } 
        public string QuestionContent { get; set; }

        public string Category { get; set; }
        public int Points { get; set; }
        public AnswerMultiplicity AnswerMultiplicity { get; set; }
        public QuestionActivityStatus ActivityStatus { get; set;}
    }
}
