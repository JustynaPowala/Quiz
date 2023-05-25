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

            foreach(var category in body.CategoriesIds ) 
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

sqlQuery +=  " ORDER BY NEWID()  COMMIT TRANSACTION";  ////

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
ORDER BY NEWID()
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
        public List<TestQuestionAnswerBody> GetListOfQuestionAnswers([FromRoute] Guid testID,[FromRoute] Guid questionID)
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

                            var answer = new  TestQuestionAnswerBody();
                   
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
        public void GetTestStartDateTime([FromRoute] Guid testID)
        {
            string connectionString = GetConnectionString();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.ConnectionString = connectionString;

                connection.Open();

                string sqlQuery = @"
UPDATE dbo.Tests
SET Started = @Started
WHERE ID = @testID";

                using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                {
                    command.Parameters.AddWithValue("@testID", testID);
                    command.Parameters.AddWithValue("@Started", DateTime.Now);
                    command.ExecuteNonQuery();
                }
            }
        }


        private string GetConnectionString()
        {

            return _configuration["ConnectionStrings:Default"];
        }

        }

    }

    

