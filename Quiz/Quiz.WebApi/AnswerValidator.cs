using Quiz.Contracts;
using Quiz.WebApi;

namespace Quiz.WebApi
{
    public class AnswerValidator
    {
        public void Validate(string content)
        {
            if (string.IsNullOrEmpty(content))
            {
                throw new DomainValidationException("Answer content field is required");
            }
        }
    }
}
