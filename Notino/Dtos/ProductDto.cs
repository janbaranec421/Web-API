using System.ComponentModel.DataAnnotations;

namespace Notino.Dtos
{
    public class ProductDto
    {
        [Key]
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
    }
}
