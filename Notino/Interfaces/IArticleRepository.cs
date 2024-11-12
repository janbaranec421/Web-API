using Notino.Dtos;
using Notino.Models;

namespace Notino.Interfaces
{
    public interface IArticleRepository
    {
        public Task<PagedResponse<Article>> GetArticlesAsync(int pageIndex, int pageSize);
        public Task<Article> GetArticleAsync(int id);
        public Task<Article> GetArticleTrimToLowerAsync(ArticleDto articleDto);
        public Task<ICollection<Product>> GetProductsByArticleAsync(int articleId);
        public Task<bool> CreateArticleAsync(Article article);
        public Task<bool> UpdateArticleAsync(Article article);
        public Task<bool> DeleteArticleAsync(Article article);
        public Task<bool> ArticleExistsAsync(int id);
        public Task<bool> SaveAsync();
    }
}
