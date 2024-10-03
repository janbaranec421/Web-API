using Microsoft.EntityFrameworkCore;
using Notino.Models;

namespace Notino.Data
{
    public class ApiDbContext : DbContext
    {
        public DbSet<Article> Articles { get; set; }
        public DbSet<Product> Products { get; set; }

        public ApiDbContext(DbContextOptions<ApiDbContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
