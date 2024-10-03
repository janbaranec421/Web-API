using Microsoft.EntityFrameworkCore;
using Notino.Data;
using Notino.Dtos;
using Notino.Interfaces;
using Notino.Models;

namespace Notino.Repositories
{
    public class ArticleRepository : IArticleRepository
    {
        private readonly ApiDbContext _context;

        public ArticleRepository(ApiDbContext context)
        {
            _context = context;
        }

        public async Task<ICollection<Article>> GetArticles()
        {
            return await _context.Articles.Include(x => x.Products).OrderBy(x => x.Id).ToListAsync();
        }

        public async Task<Article> GetArticleTrimToLower(ArticleDto articleDto)
        {
            return await _context.Articles.Where(x => x.Title.Trim().ToLower() == articleDto.Title.Trim().ToLower()).FirstOrDefaultAsync();

        }

        public async Task<Article> GetArticle(int id)
        {
            return await _context.Articles.Where(x => x.Id == id).Include(x => x.Products).FirstOrDefaultAsync();
        }

        public async Task<ICollection<Product>> GetProductsByArticle(int articleId)
        {
            return await _context.Products.Where(x => x.Article.Id == articleId).ToListAsync();
        }
        public async Task<bool> CreateArticle(Article article)
        {
            _context.Add(article);
            return await Save();
        }

        public async Task<bool> UpdateArticle(Article article)
        {
            _context.Update(article);
            return await Save();
        }

        public async Task<bool> DeleteArticle(Article article)
        {
            _context.Remove(article);
            return await Save();
        }

        public async Task<bool> ArticleExists(int id)
        {
            return await _context.Articles.AnyAsync(e => e.Id == id);
        }

        public async Task<bool> Save()
        {
            var saved = await _context.SaveChangesAsync();
            return saved > 0;
        }
    }
}
