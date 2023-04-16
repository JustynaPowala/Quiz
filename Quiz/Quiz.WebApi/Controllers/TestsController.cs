using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;


namespace Quiz.WebApi.Controllers
{
    [ApiController]
    [Route("tests")]
    public class TestsController : ControllerBase
    {


        private readonly ILogger<TestsController> _logger;

        public TestsController(ILogger<TestsController> logger)
        {
            _logger = logger;

        }

        [HttpGet("random-number")]
        public int GetRandomNumber()
        {
            return new Random().Next(0, 100);

        }

        [HttpGet("random-guid")]
        public Guid GetRandomGuid()
        {
            return Guid.NewGuid();
        }


        private string _x;




        //public string OpenSqlConnection()
        //{
        //    string connectionString = GetConnectionString();

        //    using (SqlConnection connection = new SqlConnection(connectionString))
        //    {
        //        connection.ConnectionString = connectionString;

        //        connection.Open();

        //        string sqlQuery = "SELECT * FROM dbo.Questions";

        //        using (SqlCommand command = new SqlCommand(sqlQuery, connection))
        //        {
        //            using (SqlDataReader reader = command.ExecuteReader())
        //            {
        //                while (reader.Read())
        //                {
        //                    // Retrieve data from the reader and process as needed


        //                    string questionId = reader["ID"].ToString();
        //                    questionId ??= string.Empty;

        //                    string questionText = reader["QuestionContent"].ToString();
        //                    questionText ??= string.Empty;
        //                    Console.WriteLine($"ID: {questionId}, QuestionText: {questionText}");
        //                    _x = questionId + " " + questionText;



        //                }
        //            }
        //        }
        //    }
        //    return _x;
        //}

        
        private string _guid;
        private string _question;
        [HttpPost("to-database")]
        public void Inserting(string Guid, string Ques)
        {
            _guid = Guid;
            _question = Ques;

            string connectionString = GetConnectionString();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.ConnectionString = connectionString;

                connection.Open();

                string sqlQuery = "INSERT INTO dbo.Questions (Id, QuestionContent) VALUES (@Id, @QuestionContent)";

                using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                {
                    command.Parameters.AddWithValue("@Id", _guid);
                    command.Parameters.AddWithValue("@QuestionContent", _question);
                    command.ExecuteNonQuery();
                }
            }
        }




        public static string GetConnectionString()
            {
            // To avoid storing the connection string in your code,
            // you can retrieve it from a configuration file.
            return "Server=DESKTOP-K74FN9P\\SQLEXPRESS;Database=Quiz;"
                + "Integrated Security=true;Encrypt=false;";
        }
    }

  


   
}