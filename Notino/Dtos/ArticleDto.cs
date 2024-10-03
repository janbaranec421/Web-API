using System.ComponentModel.DataAnnotations;

namespace Notino.Dtos
{
    public class ArticleDto
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Content { get; set; }

    }
}
