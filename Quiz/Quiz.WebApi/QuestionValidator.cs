using Quiz.Contracts;
using Quiz.WebApi;

namespace Quiz.WebApi
{
    public class QuestionValidator
    {

        public void Validate(string content, string category, int points, AnswerMultiplicity answerMultiplicity, List<CategoryInfo> listOfCategories)
        {

            if (string.IsNullOrEmpty(content))
            {
                throw new DomainValidationException("Question content field is required");
            }
            var categories =  listOfCategories;
            if (!categories.Any(c => c.ID ==category))
            {
                throw new DomainValidationException($"The {category} category is not recognized");
            }
            if (points<= 0 || points > 10)
            {
                throw new DomainValidationException("The amount of points must be between 1-10");
            }
            if (!(answerMultiplicity is AnswerMultiplicity.Single or AnswerMultiplicity.Multiple))
            {
                throw new DomainValidationException("Unrecognized answer multiplicity");
            }
        }
    }
}
