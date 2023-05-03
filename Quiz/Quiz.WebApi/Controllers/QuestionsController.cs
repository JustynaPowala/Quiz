using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Quiz.Contracts;
using Syncfusion.Blazor;
using System;
using System.Security.Cryptography.Xml;

namespace Quiz.WebApi.Controllers
{
    [ApiController]
    [Route("questions")]
    public class QuestionsController : ControllerBase
    {


        private readonly ILogger<QuestionsController> _logger;
        private readonly IConfiguration _configuration;
        private readonly ICategoriesProvider _categoriesProvider;

        public QuestionsController(ILogger<QuestionsController> logger, IConfiguration configuration, ICategoriesProvider categoriesProvider)
        {
            _logger = logger;
            _configuration = configuration;
            _categoriesProvider = categoriesProvider;
        }

        [HttpPost("")]
        public async Task<Guid> AddQuestionAsync([FromBody] AddQuestionBody body)
        {
            var categories = await _categoriesProvider.GetCategoriesAsync();
            var validator = new QuestionValidator(body.QuestionContent, body.Category, body.Points, body.SelectionMultiplicity, categories);
            //if (string.IsNullOrEmpty(body.QuestionContent))
            //{
            //    throw new DomainValidationException("Question content field is required");
            //}
            //var categories = await _categoriesProvider.GetCategoriesAsync();
            //if (!categories.Any(c => c.ID == body.Category))
            //{
            //    throw new DomainValidationException($"The {body.Category} category is not recognized");
            //}
            //if (body.Points <= 0 || body.Points > 10)
            //{
            //    throw new DomainValidationException("The amount of points must be between 1-10");
            //}
            //if (!(body.SelectionMultiplicity is AnswerMultiplicity.Single or AnswerMultiplicity.Multiple ))
            //{
            //    throw new DomainValidationException("Unrecognized answer multiplicity");
            //}
            var id =  Guid.NewGuid();

            string connectionString = GetConnectionString();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.ConnectionString = connectionString;

                connection.Open();

                string sqlQuery = "INSERT INTO dbo.Questions (Id, QuestionContent, Points, Category, SelectionMultiplicity) VALUES (@Id, @QuestionContent, @Points, @Category, @SelectionMultiplicity)";

                using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    command.Parameters.AddWithValue("@QuestionContent", body.QuestionContent);
                    command.Parameters.AddWithValue("@Points", body.Points);
                    command.Parameters.AddWithValue("@Category", body.Category);
                    command.Parameters.AddWithValue("@SelectionMultiplicity", body.SelectionMultiplicity.ToString());
                    command.ExecuteNonQuery();
                }
            }
            return id;
        }

        [HttpPost("{questionID}/answers")]
        public Guid AddQuestionAnswer([FromRoute] Guid questionID, [FromBody] AddAnswerBody body)
        {
            var answerId = Guid.NewGuid();

            string connectionString = GetConnectionString();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.ConnectionString = connectionString;

                connection.Open();

                string sqlQuery = "INSERT INTO dbo.Answers (QuestionID, ID, AnswerContent, IsCorrect, CreationTimestamp) VALUES (@QuestionID, @ID, @AnswerContent, @IsCorrect, @CreationTimestamp)";

                using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                {
                    command.Parameters.AddWithValue("@QuestionID", questionID);
                    command.Parameters.AddWithValue("@Id", answerId);
                    command.Parameters.AddWithValue("@AnswerContent", body.AnswerContent);
                    command.Parameters.AddWithValue("@IsCorrect", body.IsCorrect ?1:0);         //jeœli to co przed? bêdzie true to przyjmie to co przed: a jak false to to co po:. czyli 1 bêdzie true a 0 bêdzie false- tak jak jst w sql.
                    command.Parameters.AddWithValue("@CreationTimestamp", DateTime.Now);
                    command.ExecuteNonQuery();
                }
            }
            return answerId;
        }


        [HttpDelete("{questionID}/answers/{answerID}")]
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


        [HttpDelete("{questionID}")]      
        public void DeletingQuestion([FromRoute] Guid questionID)
        {
            string connectionString = GetConnectionString();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.ConnectionString = connectionString;

                connection.Open();

                string sqlQuery = @"
BEGIN TRANSACTION
BEGIN TRY
DELETE FROM dbo.Answers WHERE QuestionID = @ID
DELETE FROM dbo.Questions WHERE ID = @ID
COMMIT TRANSACTION
END TRY
BEGIN CATCH
ROLLBACK TRANSACTION
END CATCH";

                using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                {
                    command.Parameters.AddWithValue("@ID", questionID);
                    command.ExecuteNonQuery();
                }
            }

        }

        [HttpGet("{questionID}/answers")]
        public List<GetAllInfosAboutAnswer> ViewListOfAnswers([FromRoute] Guid questionID)
        {
            var listOfAnswers = new List<GetAllInfosAboutAnswer>();

            string connectionString = GetConnectionString();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.ConnectionString = connectionString;

                connection.Open();

                string sqlQuery = "SELECT * FROM dbo.Answers where QuestionID = @questionID order by CreationTimestamp";

                using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                {
                    command.Parameters.AddWithValue("@questionID", questionID);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        
                        while (reader.Read())
                        {
                            // Retrieve data from the reader and process as needed
                            var answer = new GetAllInfosAboutAnswer();

                            string questionId = reader["questionID"].ToString();
                            questionId ??= string.Empty;

                            answer.QeGuid = Guid.Parse(questionId);

                            string answerId = reader["ID"].ToString();
                            answerId ??= string.Empty;

                            answer.AnswGuid = Guid.Parse(answerId);


                            string answerText = reader["AnswerContent"].ToString();
                            answerText ??= string.Empty;
                            answer.AnswerContent = answerText;

                            string isCorrect = reader["IsCorrect"].ToString();
                            isCorrect ??= string.Empty;
                            if (isCorrect == "False")
                            {
                                answer.IsCorrect = false;
                            }
                            else if (isCorrect == "True")
                            {
                                answer.IsCorrect = true;
                            }
                            

                            listOfAnswers.Add(answer);


                        }
                    }
                }
            }
            return listOfAnswers;
        }


        [HttpGet("")]
        public List<QuestionInfo> GetListOfQuestions([FromQuery] string? category, [FromQuery] string? searchString, [FromQuery] int skipCount, [FromQuery] int maxResultCount)
        {
            var listOfQuestions = new List<QuestionInfo>();
            

            string connectionString = GetConnectionString();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.ConnectionString = connectionString;

                connection.Open();
                var sqlQuery = "";

                if(searchString != null) { 
                 sqlQuery = "SELECT * FROM dbo.Questions where Category = @category and QuestionContent like '%' + @searchString + '%' ORDER BY QuestionContent OFFSET @skipCount ROWS FETCH NEXT @maxResultCount ROWS ONLY";
                }
                else
                {
                    sqlQuery = "SELECT * FROM dbo.Questions where Category = @category ORDER BY QuestionContent OFFSET @skipCount ROWS FETCH NEXT @maxResultCount ROWS ONLY";
                }

                using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                {
                    command.Parameters.AddWithValue("@category", category);
                    command.Parameters.AddWithValue("@skipCount", skipCount);
                    command.Parameters.AddWithValue("@maxResultCount", maxResultCount);
                    if (searchString != null)
                    {
                        command.Parameters.AddWithValue("@searchString", searchString);
                       

                    }

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var question = new QuestionInfo();

                            string questionId = reader["ID"].ToString();
                            questionId ??= string.Empty;
                            question.Guid = Guid.Parse(questionId);


                            string questionText = reader["QuestionContent"].ToString();
                            questionText ??= string.Empty;
                            question.QuestionContent = questionText;

                            string quesCategory = reader["Category"].ToString();
                            quesCategory ??= string.Empty;
                            question.Category = quesCategory;
               
                            listOfQuestions.Add(question);                      
                        }
                    }
                }
            }
            return listOfQuestions;
        }

        [HttpPut("{questionID}")]
        public async Task ModifyQuestion([FromRoute] Guid questionID, [FromBody] AddQuestionBody body)
        {
            var categories = await _categoriesProvider.GetCategoriesAsync();
            var validator = new QuestionValidator(body.QuestionContent, body.Category, body.Points, body.SelectionMultiplicity, categories);

            string connectionString = GetConnectionString();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.ConnectionString = connectionString;

                connection.Open();

                string sqlQuery = @"
UPDATE dbo.Questions 
SET QuestionContent = @QuestionContent, 
    Points = @Points, 
    Category = @Category, 
    SelectionMultiplicity = @SelectionMultiplicity 
WHERE Id = @Id";

            
                using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                {
                    command.Parameters.AddWithValue("@Id", questionID);
                    command.Parameters.AddWithValue("@QuestionContent", body.QuestionContent);
                    command.Parameters.AddWithValue("@Points", body.Points);
                    command.Parameters.AddWithValue("@Category", body.Category);
                    command.Parameters.AddWithValue("@SelectionMultiplicity", body.SelectionMultiplicity.ToString());
                    command.ExecuteNonQuery();
                }
            }
        }
            


        private  string GetConnectionString()
            {
            
            return _configuration["ConnectionStrings:Default"];    // appsettings, tam jest path
             }
    }

  


   
}