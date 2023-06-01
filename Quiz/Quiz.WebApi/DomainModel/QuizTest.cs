namespace Quiz.WebApi.DomainModel
{
    public class QuizTest
    {
        public List<TestQuestion> Questions { get; set; } = new List<TestQuestion>();
        public double GetGainedPoints()
        {
            var sum = 0.0;
            foreach (var item in Questions)
            {
                sum = sum + item.GetGainedPoints();
            }
            return sum;
        }

        public double GetMaxPointsToGain()
        {
            var sum = 0.0;
            foreach (var item in Questions)
            {
                sum = sum + item.Points;
            }
            return sum;
        }
    }
}
