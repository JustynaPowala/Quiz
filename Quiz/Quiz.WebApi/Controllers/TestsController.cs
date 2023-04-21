using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Quiz.Contracts;
using System.Security.Cryptography.Xml;

namespace Quiz.WebApi.Controllers
{
    [ApiController]
    [Route("tests")]
    public class TestsController : ControllerBase
    {


        private readonly ILogger<TestsController> _logger;
        private readonly IConfiguration _configuration;

        public TestsController(ILogger<TestsController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        [HttpGet("random-number")]
        public int GetRandomNumber()
        {
            return new Random().Next(0, 100);

        }

        [HttpGet("random-guid")]
        public Guid GetRandomGuid()
        {
            _logger.LogCritical("abc");
             return Guid.NewGuid();
        }


      


        [HttpGet("top-five")]
        public List<GetQuestionIdAndContent> OpenSqlConnection()
        {
           var  listOfTop5 = new List<GetQuestionIdAndContent>();
            
            string connectionString = GetConnectionString();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.ConnectionString = connectionString;

                connection.Open();

                string sqlQuery = "SELECT TOP 5 * FROM dbo.Questions";

                using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            // Retrieve data from the reader and process as needed
                            var question = new GetQuestionIdAndContent();

                            string questionId = reader["ID"].ToString();
                            questionId ??= string.Empty;
                            
                            question.Guid = Guid.Parse(questionId);


                            string questionText = reader["QuestionContent"].ToString();
                            questionText ??= string.Empty;
                            question.QuestionContent = questionText;
                            
                            listOfTop5.Add(question);


                        }
                    }
                }
            }
            return listOfTop5;
        }



        [HttpPost("questions")] //
        public Guid AddQuestion([FromBody] AddQuestionBody body)
        {
            var guid =  GetRandomGuid();

            string connectionString = GetConnectionString();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.ConnectionString = connectionString;

                connection.Open();

                string sqlQuery = "INSERT INTO dbo.Questions (Id, QuestionContent, Points, Category, SelectionMultiplicity) VALUES (@Id, @QuestionContent, @Points, @Category, @SelectionMultiplicity)";

                using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                {
                    command.Parameters.AddWithValue("@Id", guid);
                    command.Parameters.AddWithValue("@QuestionContent", body.QuestionContent);
                    command.Parameters.AddWithValue("@Points", body.Points);
                    command.Parameters.AddWithValue("@Category", body.Category);
                    command.Parameters.AddWithValue("@SelectionMultiplicity", body.SelectionMultiplicity);
                    command.ExecuteNonQuery();
                }
            }
            return guid;
        }

        [HttpPost("questions/{questionID}/answers")]
        public Guid AddQuestionAnswer([FromRoute] Guid questionID, [FromBody] AddAnswerBody body)
        {
            var answerGuid = GetRandomGuid();

            string connectionString = GetConnectionString();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.ConnectionString = connectionString;

                connection.Open();

                string sqlQuery = "INSERT INTO dbo.Answers (QuestionID, ID, AnswerContent, IsCorrect) VALUES (@QuestionID, @ID, @AnswerContent, @IsCorrect)";

                using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                {
                    command.Parameters.AddWithValue("@QuestionID", questionID);
                    command.Parameters.AddWithValue("@Id", answerGuid);
                    command.Parameters.AddWithValue("@AnswerContent", body.AnswerContent);
                    command.Parameters.AddWithValue("@IsCorrect", body.IsCorrect ?1:0);         //jeœli to co przed? bêdzie true to przyjmie to co przed: a jak false to to co po:. czyli 1 bêdzie true a 0 bêdzie false- tak jak jst w sql.
                    command.ExecuteNonQuery();
                }
            }
            return answerGuid;
        }


        [HttpDelete("questions/{questionID}/answers/{answerID}")]
        public void  DeletingAnswer([FromRoute] Guid questionID, Guid answerID)
        {

            string connectionString = GetConnectionString();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.ConnectionString = connectionString;

                connection.Open();

                string sqlQuery = "DELETE FROM dbo.Answers WHERE ID = @ID";

                using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                {
                    command.Parameters.AddWithValue("@ID", answerID);
                    command.ExecuteNonQuery();
                }
            }
            
        }


        [HttpPost("execute-test/{x1}/tralala/{x2}/abc")]
        public TestResponseBody ExecuteTest([FromRoute]int x1,  [FromRoute]int x2, [FromQuery] int y1, [FromQuery] int y2, [FromBody] TestRequestBody body)
        {
            return new TestResponseBody()
            {
                ResultA = x1 * x2 * y1 * y2 * body.NumberA * body.NumberB,
                ResultC = (x1 * y1 * body.NumberA).ToString(),
            };
        }



        private  string GetConnectionString()
            {
            
            return _configuration["ConnectionStrings:Default"];    // appsettings, tam jest path
        }
    }

  


   
}