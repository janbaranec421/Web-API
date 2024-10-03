using System.ComponentModel.DataAnnotations;

namespace Notino.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public Article? Article { get; set; }
    }
}
