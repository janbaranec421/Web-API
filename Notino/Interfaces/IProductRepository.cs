using Notino.Dtos;
using Notino.Models;

namespace Notino.Interfaces
{
    public interface IProductRepository
    {
        public Task<PagedResponse<Product>> GetProducts(int pageIndex, int pageSize);
        public Task<Product> GetProduct(int id);
        public Task<Product> GetProductTrimToLower(ProductDto ProductDto);
        public Task<bool> CreateProduct(Product product);
        public Task<bool> UpdateProduct(Product product);
        public Task<bool> DeleteProduct(Product product);
        public Task<bool> ProductExists(int id);
        public Task<bool> Save();
    }
}
