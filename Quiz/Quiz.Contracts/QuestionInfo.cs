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
        }
       public QuestionInfo(Guid guid, string questionContent, string category)  // 2 różne przeciążenia tej samej metody
        {
            Guid = guid;
            QuestionContent = questionContent;
            Category = category;
        }
        public Guid Guid { get; set; } 
        public string QuestionContent { get; set; }

        public string Category { get; set; }
    }
}
