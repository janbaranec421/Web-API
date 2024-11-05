using AutoMapper;
using Notino.Dtos;
using Notino.Models;

namespace Notino.Helpers
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {   
            // Domain --> Dto
            CreateMap<Article, ArticleDto>();
            CreateMap<Product, ProductDto>();
            // Dto --> Domain
            CreateMap<ArticleDto, Article>();
            CreateMap<ProductDto, Product>();

            CreateMap(typeof(PagedResponse<>), typeof(PagedResponseDto<>));
        }
    }
}
