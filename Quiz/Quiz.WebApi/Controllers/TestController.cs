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
                var testsCategories = string.Join(",", body.CategoriesIds);
                string sqlQuery = @"
BEGIN TRANSACTION
INSERT INTO dbo.Tests (Id, Status) VALUES (@Id, @Status)

INSERT  INTO dbo.TestQuestions(ID, QuestionID, TestID)
SELECT TOP 10 NEWID(), ID, @Id FROM dbo.Questions";

if (body.CategoriesIds.Any())
                {
                    sqlQuery += " WHERE Category IN(" + testsCategories + ") ";
                }

sqlQuery += @"

ORDER BY NEWID()

COMMIT TRANSACTION";

                using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    command.Parameters.AddWithValue("@Status", TestStatus.Generated.ToString());             
                    command.ExecuteNonQuery();
                }
            }
            return id;
        }

        [HttpGet]




        private string GetConnectionString()
        {

            return _configuration["ConnectionStrings:Default"];
        }

        }

    }

    

