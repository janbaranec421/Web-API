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

        public async Task<PagedResponse<Article>> GetArticlesAsync(int pageIndex = 1, int pageSize = 5)
        {
            var totalRecords = await _context.Articles.CountAsync();
            var articles = await _context.Articles.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
            return new PagedResponse<Article>(articles, pageIndex, pageSize, totalRecords);
        }

        public async Task<Article> GetArticleTrimToLowerAsync(ArticleDto articleDto)
        {
            return await _context.Articles.Where(x => x.Title.Trim().ToLower() == articleDto.Title.Trim().ToLower()).FirstOrDefaultAsync();

        }

        public async Task<Article> GetArticleAsync(int id)
        {
            return await _context.Articles.Where(x => x.Id == id).Include(x => x.Products).FirstOrDefaultAsync();
        }

        public async Task<ICollection<Product>> GetProductsByArticleAsync(int articleId)
        {
            return await _context.Products.Where(x => x.Article.Id == articleId).ToListAsync();
        }
        public async Task<bool> CreateArticleAsync(Article article)
        {
            _context.Add(article);
            return await SaveAsync();
        }

        public async Task<bool> UpdateArticleAsync(Article article)
        {
            _context.Update(article);
            return await SaveAsync();
        }

        public async Task<bool> DeleteArticleAsync(Article article)
        {
            _context.Remove(article);
            return await SaveAsync();
        }

        public async Task<bool> ArticleExistsAsync(int id)
        {
            return await _context.Articles.AnyAsync(e => e.Id == id);
        }

        public async Task<bool> SaveAsync()
        {
            var saved = await _context.SaveChangesAsync();
            return saved > 0;
        }
    }
}
