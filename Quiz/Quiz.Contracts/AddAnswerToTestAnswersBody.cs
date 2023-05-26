using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quiz.Contracts
{
    public class AddAnswerToTestAnswersBody
    {

        public AddAnswerToTestAnswersBody()
        {
            AnswGuid = Guid.Empty;
        }
        public AddAnswerToTestAnswersBody(Guid answGuid)
        {
            AnswGuid = answGuid;        
        }

        public Guid AnswGuid { get; set; }
    }



}

