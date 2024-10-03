using Microsoft.EntityFrameworkCore;
using Notino.Data;
using Notino.Dtos;
using Notino.Interfaces;
using Notino.Models;

namespace Notino.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly ApiDbContext _context;

        public ProductRepository(ApiDbContext context)
        {
            _context = context;
        }

        public async Task<ICollection<Product>> GetProducts()
        {
            return await _context.Products.OrderBy(x => x.Id).ToListAsync();
        }

        public async Task<Product> GetProduct(int id)
        {
            return await _context.Products.Where(x => x.Id == id).FirstOrDefaultAsync();
        }

        public async Task<Product> GetProductTrimToLower(ProductDto productDto)
        {
            return await _context.Products.Where(x => x.Name.Trim().ToLower() == productDto.Name.Trim().ToLower()).FirstOrDefaultAsync();
        }

        public async Task<bool> CreateProduct(Product product)
        {
            _context.Add(product);
            return await Save();
        }
        public async Task<bool> UpdateProduct(Product product)
        {
            _context.Update(product);
            return await Save();
        }

        public async Task<bool> ProductExists(int id)
        {
            return await _context.Products.AnyAsync(e => e.Id == id);
        }

        public async Task<bool> DeleteProduct(Product product)
        {
            _context.Remove(product);
            return await Save();
        }

        public async Task<bool> Save()
        {
            var saved = await _context.SaveChangesAsync();
            return saved > 0;
        }
    }
}
