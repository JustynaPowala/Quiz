using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Quiz.Contracts;
using Syncfusion.Blazor;

namespace Quiz.WebApi.Controllers
{

    [ApiController]
    [Route("tests")]
    public class TestController : Controller
    {
        private readonly ICategoriesProvider _categoriesProvider;
        private readonly IConfiguration _configuration;
        public TestController(IConfiguration configuration, ICategoriesProvider categoriesProvider)
        {
            _configuration = configuration;
            _categoriesProvider = categoriesProvider;
        }


        [HttpPost("")]
        public async Task<Guid> CreateTest([FromBody] CreateTestBody body)
        {
            var categories = await _categoriesProvider.GetCategoriesAsync();

            foreach (var category in body.CategoriesIds)
            {
                if (!categories.Any(c => c.ID == category))
                {
                    throw new DomainValidationException($"The {category} category is not recognized");
                }


            }
            var id = Guid.NewGuid();

            string connectionString = GetConnectionString();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.ConnectionString = connectionString;

                connection.Open();
                //var testsCategories = string.Join(",",body.CategoriesIds);
                var testsCategories2 = body.CategoriesIds.Select(x => "'" + x + "'").ToList();
                var testsCategories = string.Join(",", testsCategories2);

                string sqlQuery = @"
BEGIN TRANSACTION
INSERT INTO dbo.Tests (ID, Status) VALUES (@Id, @Status)

INSERT INTO dbo.TestQuestions(ID, QuestionID, TestID)
SELECT TOP 10 NEWID(), ID, @Id FROM dbo.Questions
WHERE Status = @Status2";


                if (body.CategoriesIds.Any())
                {
                    sqlQuery += " AND Category IN(" + testsCategories + ") ";
                }

                sqlQuery += " ORDER BY NEWID()  COMMIT TRANSACTION";  ////

                using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    command.Parameters.AddWithValue("@Status", TestStatus.Generated.ToString());
                    command.Parameters.AddWithValue("@Status2", QuestionActivityStatus.Active.ToString());
                    command.ExecuteNonQuery();
                }
            }
            return id;
        }

        [HttpGet("{testID}/questions")]
        public TestQuestionBody GetTestQuestion([FromRoute] Guid testID, [FromQuery] int skipCount)
        {
            var TestQ = new TestQuestionBody();
            string connectionString = GetConnectionString();


            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.ConnectionString = connectionString;

                connection.Open();

                string sqlQuery = @"

SELECT Q.QuestionContent,Points,SelectionMultiplicity,Q.ID
FROM  dbo.Questions as Q 
inner join dbo.TestQuestions as TQ
on TQ.QuestionID = Q.ID
WHERE TQ.TestID = @testID
ORDER BY Q.QuestionContent
OFFSET @skipCount ROWS FETCH NEXT 1 ROWS ONLY";
                using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                {
                    command.Parameters.AddWithValue("@testID", testID);
                    command.Parameters.AddWithValue("@skipCount", skipCount);


                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {

                            string questionContent = reader["QuestionContent"].ToString();
                            questionContent ??= string.Empty;
                            TestQ.QuestionContent = questionContent;

                            string points = reader["Points"].ToString();
                            points ??= string.Empty;
                            TestQ.Points = int.Parse(points);

                            string answerMultiplicity = reader["SelectionMultiplicity"].ToString();
                            answerMultiplicity ??= string.Empty;

                            TestQ.AnswerMultiplicity = Enum.Parse<AnswerMultiplicity>(answerMultiplicity);

                            string QuesId = reader["ID"].ToString();
                            QuesId ??= string.Empty;

                            TestQ.QGuid = Guid.Parse(QuesId);


                        }
                    }
                }
                return TestQ;

            }
        }

        [HttpGet("{testID}/questions/{questionID}/answers")]
        public List<TestQuestionAnswerBody> GetListOfQuestionAnswers([FromRoute] Guid testID, [FromRoute] Guid questionID)
        {
            var listOfAnswers = new List<TestQuestionAnswerBody>();

            string connectionString = GetConnectionString();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.ConnectionString = connectionString;

                connection.Open();

                string sqlQuery = @"
SELECT A.*, TA.AnswerID
FROM dbo.Answers as A
INNER JOIN dbo.Questions AS Q
on A.QuestionID = Q.ID
INNER JOIN dbo.TestQuestions as TQ
on Q.ID = TQ.QuestionID
LEFT JOIN dbo.TestAnswers as TA
on TQ.ID = TA.TestQuestionsID and A.ID = TA.AnswerID
where A.QuestionID = @questionID AND TQ.TestID = @testID
order by A.CreationTimestamp";

                using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                {
                    command.Parameters.AddWithValue("@testID", testID);
                    command.Parameters.AddWithValue("@questionID", questionID);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {

                        while (reader.Read())
                        {

                            var answer = new TestQuestionAnswerBody();

                            string answerId = reader["ID"].ToString();
                            answerId ??= string.Empty;

                            answer.AnswGuid = Guid.Parse(answerId);


                            string answerText = reader["AnswerContent"].ToString();
                            answerText ??= string.Empty;
                            answer.AnswerContent = answerText;

                            string isSelected = reader["AnswerID"].ToString();

                            if (isSelected == string.Empty)
                            {
                                answer.IsSelected = false;
                            }
                            else
                            {
                                answer.IsSelected = true;
                            }


                            listOfAnswers.Add(answer);
                        }
                    }
                }
            }
            return listOfAnswers;
        }





        [HttpGet("{testID}/questions/count")]  // this method is needed when there would be less than 10 possible questions to be drawn in the database.
        public int GetQuestionsCount([FromRoute] Guid testID)
        {
            string connectionString = GetConnectionString();
            var count = 0;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.ConnectionString = connectionString;

                connection.Open();

                string sqlQuery = @"

SELECT COUNT(*)
FROM  dbo.Questions as Q 
inner join dbo.TestQuestions as TQ
on TQ.QuestionID = Q.ID
WHERE TQ.TestID = @testID";
                using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                {
                    command.Parameters.AddWithValue("@testID", testID);
                    count = (int)command.ExecuteScalar();
                }
            }
            return count;
        }



        [HttpPut("{testID}/start")]
        public void StartTest([FromRoute] Guid testID)
        {
            string connectionString = GetConnectionString();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.ConnectionString = connectionString;

                connection.Open();

                string sqlQuery = @"
UPDATE dbo.Tests
SET Started = @Started, Status = @Status
WHERE ID = @testID";

                using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                {
                    command.Parameters.AddWithValue("@testID", testID);
                    command.Parameters.AddWithValue("@Started", DateTime.Now);
                    command.Parameters.AddWithValue("@Status", TestStatus.Started.ToString());
                    command.ExecuteNonQuery();
                }
            }
        }

        [HttpPost("{testID}/test-questions/{testQuestionID}/test-answers")]
        public Guid AddAnswerToTestAnswers([FromRoute] Guid testID, [FromRoute] Guid testQuestionID, [FromBody] AddAnswerToTestAnswersBody body)
        {
            var id = Guid.NewGuid();

            string connectionString = GetConnectionString();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.ConnectionString = connectionString;

                connection.Open();
              
                string sqlQuery1 = "SELECT SelectionMultiplicity FROM dbo.Questions WHERE ID = @questionID";

                using (SqlCommand command1 = new SqlCommand(sqlQuery1, connection))
                {
                    command1.Parameters.AddWithValue("@questionID", testQuestionID);
                    string selectionMultiplicity = command1.ExecuteScalar()?.ToString();

                    if (selectionMultiplicity == "Single")
                    {
                        string sqlQuery2 = @"
DELETE TA FROM dbo.TestAnswers as TA
INNER JOIN dbo.TestQuestions as TQ on TA.TestQuestionsID = TQ.ID
WHERE TQ.testID = @testID and TQ.QuestionID = @questionID";

                        using (SqlCommand command2 = new SqlCommand(sqlQuery2, connection))
                        {
                            command2.Parameters.AddWithValue("@testID", testID);
                            command2.Parameters.AddWithValue("@questionID", testQuestionID);
                            command2.ExecuteNonQuery();
                        }
                    }
                    
                    string sqlQuery3 = @"

INSERT INTO dbo.TestAnswers(ID, AnswerID, TestQuestionsID)
SELECT @Id, @answerID, ID FROM dbo.TestQuestions
WHERE testID = @testID and QuestionID = @questionID";

                    using (SqlCommand command3 = new SqlCommand(sqlQuery3, connection))
                    {
                        command3.Parameters.AddWithValue("@Id", id);
                        command3.Parameters.AddWithValue("@answerID", body.AnswGuid);
                        command3.Parameters.AddWithValue("@testID", testID);
                        command3.Parameters.AddWithValue("@questionID", testQuestionID);
                        command3.ExecuteNonQuery();
                    }

                }
            }
            return id;  /// 
        }

        [HttpDelete("{testID}/test-questions/{testQuestionID}/test-answers/{answerID}")]
        public void DeleteAnswerFromTestAnswers([FromRoute] Guid testID, [FromRoute] Guid testQuestionID, [FromRoute] Guid answerID)  // method that will be used for multiple choice questions when unchecking the checkbox
        {
            string connectionString = GetConnectionString();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.ConnectionString = connectionString;

                connection.Open();

                string sqlQuery = @"
DELETE TA FROM dbo.TestAnswers as TA
INNER JOIN dbo.TestQuestions as TQ on TA.TestQuestionsID = TQ.ID
WHERE TQ.testID = @testID and TQ.QuestionID = @questionID and TA.AnswerID = @answerID";

                using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                {
                    command.Parameters.AddWithValue("@testID", testID);
                    command.Parameters.AddWithValue("@questionID", testQuestionID);
                    command.Parameters.AddWithValue("@answerID", answerID);
                    command.ExecuteNonQuery();
                }

            }
        }

        [HttpGet("{testID}/result")]
        public TestResultAndTestMaxBody EndTest([FromRoute] Guid testID)
        {
            var maxPointsToAchieve = 0;
            var listOfQuestions = new List<Guid>();
            double sumOfAchievedPoints = 0;
            var testResultAndTestMaxBody = new TestResultAndTestMaxBody();
            var listOfSelectedAnswersInMultipleQuestion = new List<Guid>();

            string connectionString = GetConnectionString();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.ConnectionString = connectionString;

                connection.Open();

                string sqlQuery1 = @"
SELECT SUM(Q.Points) as p from dbo.TestQuestions as TQ
INNER JOIN dbo.Questions as Q on  TQ.QuestionID = Q.ID
WHERE TQ.testID = @testID
";
                using (SqlCommand command1 = new SqlCommand(sqlQuery1, connection))
                {
                    command1.Parameters.AddWithValue("@testID", testID);
                    maxPointsToAchieve = int.Parse(command1.ExecuteScalar()?.ToString()); // ?
                    testResultAndTestMaxBody.TestMaxPointsToAchieve = maxPointsToAchieve;
                }

                string sqlQuery2 = @"
SELECT QuestionID FROM dbo.TestQuestions
WHERE testID = @testID
";

                using (SqlCommand command2 = new SqlCommand(sqlQuery2, connection))
                {
                    command2.Parameters.AddWithValue("@testID", testID);
                    using (SqlDataReader reader = command2.ExecuteReader())
                    {

                        while (reader.Read())
                        {
                            string questionId = reader["QuestionID"].ToString();
                            questionId ??= string.Empty;

                            var questionIdGuid = Guid.Parse(questionId);

                            listOfQuestions.Add(questionIdGuid);
                        }
                    }

                    foreach (var question in listOfQuestions)
                    {

                        var pointsToAchieveInGivenQuestion = 0.0;
                        var achievedPointsInGivenQuestion = 0.0;
                        var pointsToAchievePerCorrectAnswer = 0.0;
                        var pointsToLosePerIncorrectAnswer = 0.0;

                        string sqlQuery3 = @"
SELECT SUM(Q.Points) as p from dbo.TestQuestions as TQ
INNER JOIN dbo.Questions as Q on  TQ.QuestionID = Q.ID
WHERE Q.ID = @questionID and TQ.testID = @testID";
                        using (SqlCommand command3 = new SqlCommand(sqlQuery3, connection))
                        {
                            command3.Parameters.AddWithValue("@testID", testID);
                            command3.Parameters.AddWithValue("@questionID", question);
                            pointsToAchieveInGivenQuestion = double.Parse(command3.ExecuteScalar()?.ToString());
                        }
                        string sqlQuery4 = "SELECT SelectionMultiplicity FROM dbo.Questions WHERE ID = @questionID";

                        using (SqlCommand command4 = new SqlCommand(sqlQuery4, connection))
                        {
                            command4.Parameters.AddWithValue("@questionID", question);
                            string selectionMultiplicity = command4.ExecuteScalar()?.ToString();

                            if (selectionMultiplicity == "Single")
                            {

                                string sqlQuery5 = @"
SELECT A.IsCorrect from dbo.Answers as A
inner join dbo.TestAnswers as TA on A.ID = TA.AnswerID
inner join dbo.TestQuestions as TQ on TA.TestQuestionsID = TQ.ID
WHERE TQ.QuestionID = @questionID and TQ.testID = @testID
";
                                using (SqlCommand command5 = new SqlCommand(sqlQuery5, connection))
                                {
                                    command5.Parameters.AddWithValue("@questionID", question);
                                    command5.Parameters.AddWithValue("@testID", testID);
                                    using (SqlDataReader reader = command5.ExecuteReader())
                                    {

                                        while (reader.Read())
                                        {
                                            string isCorrect = reader["IsCorrect"].ToString();
                                            isCorrect ??= string.Empty;
                                            if (isCorrect == "True")
                                            {
                                                sumOfAchievedPoints = sumOfAchievedPoints + pointsToAchieveInGivenQuestion;
                                            }
                                            else
                                            {

                                            }
                                        }

                                    }

                                }
                            }
                            else if (selectionMultiplicity == "Multiple")
                            {
                                string sqlQuery6 = @"

SELECT COUNT(*) as sum from dbo.Questions as Q
inner join dbo.Answers as A on Q.ID = A.QuestionID
WHERE Q.ID = @questionID and A.IsCorrect = '1'";
                                using (SqlCommand command6 = new SqlCommand(sqlQuery6, connection))
                                {
                                    command6.Parameters.AddWithValue("@questionID", question);
                                    var countOfCorrectAnswersInGivenQuestion = double.Parse(command6.ExecuteScalar()?.ToString());
                                    pointsToAchievePerCorrectAnswer = pointsToAchieveInGivenQuestion / countOfCorrectAnswersInGivenQuestion;
                                    pointsToLosePerIncorrectAnswer = pointsToAchievePerCorrectAnswer;                                   
                                }

                                string sqlQuery7 = @"

SELECT TA.AnswerID from dbo.TestQuestions as TQ
inner join dbo.TestAnswers as TA on TQ.ID = TA.TestQuestionsID
WHERE TQ.testID = @testID and TQ.QuestionID = @questionID
";
                                using (SqlCommand command7 = new SqlCommand(sqlQuery7, connection))
                                {
                                    command7.Parameters.AddWithValue("@testID", testID);
                                    command7.Parameters.AddWithValue("@questionID", question);
                                    using (SqlDataReader reader = command7.ExecuteReader())
                                    {

                                        while (reader.Read())
                                        {
                                            string answerId = reader["AnswerID"].ToString();
                                            answerId ??= string.Empty;

                                            var answerIdGuid = Guid.Parse(answerId);

                                            listOfSelectedAnswersInMultipleQuestion.Add(answerIdGuid);
                                        }
                                    }

                                    foreach (var answer in listOfSelectedAnswersInMultipleQuestion)
                                    {
                                        string sqlQuery8 = @"
Select A.IsCorrect from dbo.TestQuestions as TQ
inner join dbo.TestAnswers as TA on TQ.ID = TA.TestQuestionsID
inner join dbo.Answers as A on TA.AnswerID = A.ID
WHERE TQ.testID = @testID and TQ.QuestionID = @questionID
";
                                        using (SqlCommand command8 = new SqlCommand(sqlQuery8, connection))
                                        {
                                            command8.Parameters.AddWithValue("@questionID", question);
                                            command8.Parameters.AddWithValue("@testID", testID);
                                            using (SqlDataReader reader = command8.ExecuteReader())
                                            {

                                                while (reader.Read())
                                                {
                                                    string isCorrect = reader["IsCorrect"].ToString();
                                                    isCorrect ??= string.Empty;
                                                    if (isCorrect == "True")
                                                    {
                                                        achievedPointsInGivenQuestion = achievedPointsInGivenQuestion + pointsToAchievePerCorrectAnswer;
                                                        
                                                    }
                                                    else
                                                    {
                                                        achievedPointsInGivenQuestion = achievedPointsInGivenQuestion - pointsToLosePerIncorrectAnswer;
                                                    }                                                  
                                                }
                                                if(achievedPointsInGivenQuestion < 0)
                                                {
                                                    achievedPointsInGivenQuestion = 0;
                                                }
                                                
                                                sumOfAchievedPoints = sumOfAchievedPoints + achievedPointsInGivenQuestion;

                                            }

                                        }
                                    }


                                }
                            }
                        }
                    }
                }
            }
            testResultAndTestMaxBody.TestResult = sumOfAchievedPoints;
            return testResultAndTestMaxBody;
        }



        private string GetConnectionString()
        {

            return _configuration["ConnectionStrings:Default"];
        }

    }

}



