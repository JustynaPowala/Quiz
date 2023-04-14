using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;


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


       

        [HttpGet("from-database")]
        public static void OpenSqlConnection() //creating connection string
        {
            string connectionString = GetConnectionString();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.ConnectionString = connectionString;

                connection.Open();

                string sqlQuery = "SELECT * FROM dbo.Questions";

                using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            // Retrieve data from the reader and process as needed
                            string questionId = reader["Id"].ToString();
                            string questionText = reader["QuestionContent"].ToString();
                            Console.WriteLine($"Id: {questionId}, QuestionText: {questionText}");
                        }
                    }
                }
            }
        }
        
       
        
        public static string GetConnectionString()
            {
            // To avoid storing the connection string in your code,
            // you can retrieve it from a configuration file.
            return "Data Source=DESKTOP-K74FN9P\\SQLEXPRESS;Initial Catalog=dbo.Questions;"
                + "Integrated Security=true;";
             }       
    
       
    }

   
}