using Microsoft.AspNetCore.Mvc;

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
    }
}