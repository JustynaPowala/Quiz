using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Quiz.Contracts;
using Quiz.WebApi.DomainModel;
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




        [HttpPost("{testID}/test-questions/{testQuestionID}/test-answers")]
        public Guid AddAnswerToTestAnswers([FromRoute] Guid testID, [FromRoute] Guid testQuestionID, [FromBody] AddAnswerToTestAnswersBody body)
        {
            var id = Guid.NewGuid();

            string connectionString = GetConnectionString();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.ConnectionString = connectionString;

                connection.Open();

                string sqlQuery0 = "Select Status from dbo.Tests where ID = @testID";
                using (SqlCommand command0 = new SqlCommand(sqlQuery0, connection))
                {
                    command0.Parameters.AddWithValue("@testID", testID);
                    string testStatus = command0.ExecuteScalar()?.ToString();

                    if (testStatus == TestStatus.Completed.ToString()) 
                    {
                        throw new DomainValidationException("Test is finished, you can't change answers.");
                    }
                    else
                    {

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
            }
        }

        [HttpDelete("{testID}/test-questions/{testQuestionID}/test-answers/{answerID}")]
        public void DeleteAnswerFromTestAnswers([FromRoute] Guid testID, [FromRoute] Guid testQuestionID, [FromRoute] Guid answerID)  // method that will be used for multiple choice questions when unchecking the checkbox
        {
            string connectionString = GetConnectionString();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.ConnectionString = connectionString;

                connection.Open();

                string sqlQuery0 = "Select Status from dbo.Tests where ID = @testID";
                using (SqlCommand command0 = new SqlCommand(sqlQuery0, connection))
                {
                    command0.Parameters.AddWithValue("@testID", testID);
                    string testStatus = command0.ExecuteScalar()?.ToString();

                    if (testStatus == TestStatus.Completed.ToString()) // need to check it later
                    {
                        throw new DomainValidationException("Test is finished, you can't change answers.");
                    }
                    else
                    {
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
            }
        }

        [HttpPut("{testID}/start")] //
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


        [HttpPut("{testID}/end-test")] //
        public void EndTest([FromRoute] Guid testID)
        {
            double testResult = 0.0;
            double testMaxPointsToGain = 0.0;

            string connectionString = GetConnectionString();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.ConnectionString = connectionString;

                connection.Open();

                string sqlQuery = @"
select TQ.ID as QuestionID, Q.Points, Q.SelectionMultiplicity, A.ID as AnswerID, A.IsCorrect, TA.AnswerID as ChosenAnswer from dbo.TestQuestions as TQ
inner join dbo.Questions as Q on TQ.QuestionID = Q.ID
inner join dbo.Answers as A on Q.ID = A.QuestionID
left join dbo.TestAnswers as TA on A.ID = TA.AnswerID and TA.TestQuestionsID = TQ.ID
WHERE TQ.TestID = @testID
Order by TQ.ID
";

                using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                {
                    command.Parameters.AddWithValue("@testID", testID);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        var test = new QuizTest();
                        Guid currentQuestionID = Guid.Empty;
                        TestQuestion? currentQuestion = null;
                        while (reader.Read())
                        {
                            var questionID = Guid.Parse(reader["QuestionID"].ToString() ?? String.Empty);

                            if (questionID != currentQuestionID)
                            {
                                var points = int.Parse(reader["Points"].ToString() ?? String.Empty);
                                var answerMultiplicity = Enum.Parse<AnswerMultiplicity>(reader["SelectionMultiplicity"].ToString() ?? String.Empty);
                                currentQuestion = new TestQuestion()
                                {
                                    Id = questionID,
                                    Points = points,
                                    AnswerMultiplicity = answerMultiplicity
                                };
                                test.Questions.Add(currentQuestion);
                                currentQuestionID = questionID;
                            }
                            var anwerID = Guid.Parse(reader["AnswerID"].ToString() ?? String.Empty);
                            var isCorrect = bool.Parse((reader["IsCorrect"].ToString() ?? String.Empty).ToLower());
                            var isSelected = reader["ChosenAnswer"].ToString() != String.Empty;


                            currentQuestion.Answers.Add(new TestAnswer()
                            {
                                AnswerId = anwerID,
                                IsCorrect = isCorrect,
                                IsSelected = isSelected
                            });
                        }
                        testResult = test.GetGainedPoints();
                        testMaxPointsToGain = test.GetMaxPointsToGain();

                    }

                    string sqlQuery2 = @"
UPDATE dbo.Tests
SET Completed = @Completed, Status = @Status, MaxPoints = @MaxPoints, GainedPoints = @GainedPoints
WHERE ID = @testID";

                    using (SqlCommand command2 = new SqlCommand(sqlQuery2, connection))
                    {
                        command2.Parameters.AddWithValue("@testID", testID);
                        command2.Parameters.AddWithValue("@Completed", DateTime.Now);
                        command2.Parameters.AddWithValue("@Status", TestStatus.Completed.ToString());
                        command2.Parameters.AddWithValue("@MaxPoints", testMaxPointsToGain);
                        command2.Parameters.AddWithValue("@GainedPoints", testResult);
                        command2.ExecuteNonQuery();
                    }
                }
            }
        }

        [HttpGet("{testID}/result")]
        public TestResultBody GetResult([FromRoute] Guid testID)
        {
            double testResult = 0.0;
            double testMaxPointsToGain = 0.0;
            string connectionString = GetConnectionString();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.ConnectionString = connectionString;

                connection.Open();
                string sqlQuery = @"
SELECT MaxPoints, GainedPoints FROM dbo.Tests
WHERE ID = @testID";

                using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                {
                    command.Parameters.AddWithValue("@testID", testID);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            testResult = Convert.ToDouble(reader["GainedPoints"].ToString() ?? String.Empty);
                            testMaxPointsToGain = Convert.ToDouble(reader["MaxPoints"].ToString() ?? String.Empty);
                        }

                            return new TestResultBody()
                            {
                                TestResult = testResult,
                                TestMaxPointsToGain = testMaxPointsToGain
                            };
                        }
                    }
                }
            }
        
                 
        


        private string GetConnectionString()
        {

            return _configuration["ConnectionStrings:Default"];
        }

    }

}



