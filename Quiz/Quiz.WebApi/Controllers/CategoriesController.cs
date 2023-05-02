using Microsoft.AspNetCore.Mvc;
using Quiz.Contracts;

namespace Quiz.WebApi.Controllers
{
    [ApiController]
    [Route("categories")]
    public class CategoriesController : ControllerBase
    {


        private readonly ICategoriesProvider _categoriesProvider;
        

        public CategoriesController(ICategoriesProvider categoriesProvider)
        {
            _categoriesProvider = categoriesProvider;

        }

        [HttpGet("")]
      public async Task <List<CategoryInfo>> GetCategoriesAsync()
        {
            return await _categoriesProvider.GetCategoriesAsync();
            
        }
      
    }
}

