using Quiz.Contracts;
using System.Reflection.Metadata.Ecma335;

namespace Quiz.WebApi.DomainModel
{
    public class TestQuestion
    {
        public Guid Id { get; set; }
        public int Points { get; set; } 
        public AnswerMultiplicity AnswerMultiplicity { get; set; }
        public List<TestAnswer> Answers { get; set; } = new List<TestAnswer>();


        public double GetGainedPoints()
        {

            if (AnswerMultiplicity == AnswerMultiplicity.Single)
            {
                foreach (var answer in Answers)
                {
                    if (answer.IsSelected)
                    {
                        if (answer.IsCorrect)
                        {
                            return Points;
                        }
                        else
                        {
                            return 0;
                        }
                    }                   
                }
                return 0;
            }
            else
            {
                var countOfCorrectAnswers = 0;
                foreach (var answ in Answers)
                {
                    if (answ.IsCorrect)
                    {
                        countOfCorrectAnswers += 1;
                    }
                }
                double pointsToGainPerCorrectAnswer = Convert.ToDouble(Points)/ Convert.ToDouble(countOfCorrectAnswers);
                var pointToLosePerInCorrectAnswer = pointsToGainPerCorrectAnswer;
                var gainedPoints = 0.0;
                foreach (var answer in Answers)
                {
                    if (answer.IsSelected)
                    {
                        if (answer.IsCorrect)
                        {
                            gainedPoints += pointsToGainPerCorrectAnswer;
                        }
                        else
                        {
                            gainedPoints -= pointToLosePerInCorrectAnswer;
                        }
                    }
                }
                if (gainedPoints < 0)
                {
                    return 0;
                }
                else
                {
                    return gainedPoints;
                }
            }
        }

    }
}
