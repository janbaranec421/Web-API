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

        public async Task<PagedResponse<Article>> GetArticles(int pageIndex = 1, int pageSize = 5)
        {
            var totalRecords = await _context.Articles.CountAsync();
            var articles = await _context.Articles.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
            return new PagedResponse<Article>(articles, pageIndex, pageSize, totalRecords);
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
