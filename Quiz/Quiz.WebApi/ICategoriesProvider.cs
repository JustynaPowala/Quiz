using Quiz.Contracts;

namespace Quiz.WebApi
{
    public interface ICategoriesProvider
    {
        Task <List<CategoryInfo>> GetCategoriesAsync();
    }
}
