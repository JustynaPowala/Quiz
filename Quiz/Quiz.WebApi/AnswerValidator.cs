using Quiz.Contracts;
using Quiz.WebApi;

namespace Quiz.WebApi
{
    public class AnswerValidator
    {

        public void Validate(string content, bool correctness)
        {

            if (string.IsNullOrEmpty(content))
            {
                throw new DomainValidationException("Answer content field is required");
            }
            if (!IsValidBoolean(correctness))
            {
                throw new DomainValidationException("Unrecognized correctness");
            }
        }

        private bool IsValidBoolean(bool value)
        {
            return value == true || value == false;
        }
    }
}
