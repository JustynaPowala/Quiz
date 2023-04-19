using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Quiz.Contracts;

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



        [HttpPost("to-database")]
        public Guid Inserting([FromBody] AddQuestionBody body)
        {
            var guid =  GetRandomGuid();

            string connectionString = GetConnectionString();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.ConnectionString = connectionString;

                connection.Open();

                string sqlQuery = "INSERT INTO dbo.Questions (Id, QuestionContent, Points) VALUES (@Id, @QuestionContent, @Points)";

                using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                {
                    command.Parameters.AddWithValue("@Id", guid);
                    command.Parameters.AddWithValue("@QuestionContent", body.QuestionContent);
                    command.Parameters.AddWithValue("@Points", body.Points); 
                    command.ExecuteNonQuery();
                }
            }
            return guid;
        }




        private  string GetConnectionString()
            {
            
            return _configuration["ConnectionStrings:Default"];    // appsettings, tam jest path
        }
    }

  


   
}