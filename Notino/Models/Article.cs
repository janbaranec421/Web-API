using System.ComponentModel.DataAnnotations;

namespace Notino.Models
{
    public class Article
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Content { get; set; }
        public ICollection<Product> Products { get; set; }
    }
}
