using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quiz.Contracts
{
    public class AnswerInfo
    {
       public AnswerInfo() 
        {
            QeGuid = Guid.Empty;
            AnswGuid = Guid.Empty;
            AnswerContent = String.Empty;
            IsCorrect = false;
        }
       public AnswerInfo(Guid qeGuid, Guid answGuid, string answerContent, bool isCorrect)  // 2 różne przeciążenia tej samej metody
        {
            QeGuid=qeGuid;
            AnswGuid=answGuid;
            AnswerContent=answerContent;
            IsCorrect = isCorrect;
        }
        public Guid QeGuid { get; set; } 

        public Guid  AnswGuid { get; set; } 
        public string AnswerContent { get; set; }

        public bool IsCorrect  { get; set; } 
    }
}
