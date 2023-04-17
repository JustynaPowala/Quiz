using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quiz.Contracts
{
    public class GetQuestionIdAndContent
    {
       public GetQuestionIdAndContent() 
        {
            Guid = Guid.Empty;
            QuestionContent = String.Empty; 
        }
       public GetQuestionIdAndContent(Guid guid, string questionContent)  // 2 różne przeciążenia tej samej metody
        {
            Guid = guid;
            QuestionContent = questionContent;
        }
        public Guid Guid { get; set; } 
        public string QuestionContent { get; set; }
    }
}
