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
                    sqlQuery += " WHERE Category IN(" + testsCategories + ") ";
                }

sqlQuery +=  " ORDER BY NEWID()  COMMIT TRANSACTION";

                using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    command.Parameters.AddWithValue("@Status", TestStatus.Generated.ToString());
                    command.Parameters.AddWithValue("@Status2", QuestionActivityStatus.Active.ToString());
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

                            TestQ.TestGuid = Guid.Parse(QuesId);


                        }
                    }
                }
                return TestQ;

            }
        }






//        string answerText = reader["AnswerContent"].ToString();
//        answerText ??= string.Empty;
//                            TestQA.AnswerContent = answerText;

//                            string isCorrect = reader["IsCorrect"].ToString();
//        isCorrect ??= string.Empty;
//                            if (isCorrect == "False")
//                            {
//                                TestQA.IsCorrect = false;
//                            }
//                            else if (isCorrect == "True")
//                            {
//                                TestQA.IsCorrect = true;
//                            }

//string answerId = reader["ID"].ToString();
//answerId ??= string.Empty;

//TestQA.AnswGuid = Guid.Parse(answerId);



private string GetConnectionString()
        {

            return _configuration["ConnectionStrings:Default"];
        }

        }

    }

    

