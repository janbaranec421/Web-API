using Notino.Models;

namespace Notino.Data
{
    public static class DataInitalizer
    {
        public static WebApplication Seed(this WebApplication app)
        {
            using (var scope = app.Services.CreateScope())
            {
                using var context = scope.ServiceProvider.GetRequiredService<ApiDbContext>();
                try
                {
                    context.Database.EnsureCreated();

                    var products = context.Products.FirstOrDefault();
                    if (products == null)
                    {
                        context.Products.AddRange(
                                new Product() { Name = "Perfume", Description = "Fresh scent", Article = new Article() { Title = "Special Bundle 1", Description = "Edition 2022-Q1", Content = "Winter edition products" } },
                                new Product() { Name = "Lipstick", Description = "Red color", Article = new Article() { Title = "Special Bundle 2", Description = "Edition 2022-Q2", Content = "Spring edition products" } },
                                new Product() { Name = "Shower gel", Description = "Fresh scent", Article = new Article() { Title = "Special Bundle 3", Description = "Edition 2022-Q3", Content = "Summer edition products" } },
                                new Product() { Name = "Perfume", Description = "Fruity scent", Article = new Article() { Title = "Special Bundle 4", Description = "Edition 2022-Q4", Content = "Autumn edition products" } },
                                new Product() { Name = "Lipstick", Description = "Black color", Article = new Article() { Title = "Special Bundle 5", Description = "Edition 2023-Q1", Content = "Winter edition products" } },
                                new Product() { Name = "Shower gel", Description = "Fruity scent", Article = new Article() { Title = "Special Bundle 6", Description = "Edition 2023-Q2", Content = "Spring edition products" } },
                                new Product() { Name = "Perfume", Description = "Flower scent", Article = new Article() { Title = "Special Bundle 7", Description = "Edition 2023-Q3", Content = "Summer edition products" } },
                                new Product() { Name = "Lipstick", Description = "Red color", Article = new Article() { Title = "Special Bundle 8", Description = "Edition 2023-Q4", Content = "Autumn edition products" } },
                                new Product() { Name = "Hand cream", Description = "Moisturizing", Article = new Article() { Title = "Special Bundle 9", Description = "Edition 2024-Q1", Content = "Winter edition products" } },
                                new Product() { Name = "Perfume", Description = "Wild scent", Article = new Article() { Title = "Special Bundle 10", Description = "Edition 2024-Q2", Content = "Spring edition products" } },
                                new Product() { Name = "Lipstick", Description = "Blue color" },
                                new Product() { Name = "Hand cream", Description = "For dry skin" },
                                new Product() { Name = "Perfume", Description = "Spicy scent" },
                                new Product() { Name = "Lipstick", Description = "Green color" }
                            );

                        context.SaveChanges();
                    }
                }
                catch (Exception)
                {
                    throw;
                }
                return app;
            }
        }
    }
}
