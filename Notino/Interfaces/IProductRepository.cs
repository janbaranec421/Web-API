using Notino.Dtos;
using Notino.Models;

namespace Notino.Interfaces
{
    public interface IProductRepository
    {
        public Task<PagedResponse<Product>> GetProductsAsync(int pageIndex, int pageSize);
        public Task<Product> GetProductAsync(int id);
        public Task<Product> GetProductTrimToLowerAsync(ProductDto ProductDto);
        public Task<bool> CreateProductAsync(Product product);
        public Task<bool> UpdateProductAsync(Product product);
        public Task<bool> DeleteProductAsync(Product product);
        public Task<bool> ProductExistsAsync(int id);
        public Task<bool> SaveAsync();
    }
}
