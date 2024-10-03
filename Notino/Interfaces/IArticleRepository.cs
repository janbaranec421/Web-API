using Notino.Dtos;
using Notino.Models;

namespace Notino.Interfaces
{
    public interface IArticleRepository
    {
        public Task<ICollection<Article>> GetArticles();
        public Task<Article> GetArticle(int id);
        public Task<Article> GetArticleTrimToLower(ArticleDto articleDto);
        public Task<ICollection<Product>> GetProductsByArticle(int articleId);
        public Task<bool> CreateArticle(Article article);
        public Task<bool> UpdateArticle(Article article);
        public Task<bool> DeleteArticle(Article article);
        public Task<bool> ArticleExists(int id);
        public Task<bool> Save();
    }
}
