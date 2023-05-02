using Quiz.Contracts;



namespace Quiz.WebApi
{
    public class HardcodedCategoriesProvider : ICategoriesProvider
    {
        public async Task<List<CategoryInfo>> GetCategoriesAsync()
        {
            return new List<CategoryInfo>()
            {
                new CategoryInfo() {ID="history", Name="History" },
                new CategoryInfo() {ID="chemistry", Name="Chemistry" },
                new CategoryInfo() {ID="mathematics", Name="Mathematics" },
                new CategoryInfo() {ID="music", Name="Music" },
                new CategoryInfo() {ID="information technology", Name="Information technology" }
            };
        }
    }
}
