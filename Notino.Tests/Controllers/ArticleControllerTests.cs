using AutoMapper;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Notino.Controllers;
using Notino.Dtos;
using Notino.Interfaces;
using Notino.Models;

namespace Notino.Tests.Controller
{
    public class ArticleControllerTests
    {
        private readonly IArticleRepository _articleRepository;
        private readonly IMapper _mapper;
        private readonly ICacheService _cacheService;
        private readonly ILogger<ArticleController> _logger;


        public ArticleControllerTests()
        {
            _articleRepository = A.Fake<IArticleRepository>();
            _mapper = A.Fake<IMapper>();
            _cacheService = A.Fake<ICacheService>();
            _logger = A.Fake<ILogger<ArticleController>>();
        }

        [Fact]
        public async void ArticleController_GetArticles_ReturnOK()
        {
            //Arrange
            var articles = A.Fake<ICollection<ArticleDto>>();
            var articlesList = A.Fake<List<ArticleDto>>();
            A.CallTo(() => _mapper.Map<ICollection<ArticleDto>>(articles)).Returns(articlesList);
            var controller = new ArticleController(_articleRepository, _mapper, _cacheService, _logger);

            //Act
            var result = await controller.GetArticles();

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(OkObjectResult));
        }

        [Fact]
        public async void ArticleController_GetArticle_ReturnOK()
        {
            //Arrange
            var articleId = 1;
            var article = A.Fake<Article>();
            var articleDto = A.Fake<ArticleDto>();
            A.CallTo(() => _mapper.Map<ArticleDto>(article)).Returns(articleDto);
            var controller = new ArticleController(_articleRepository, _mapper, _cacheService, _logger);

            //Act
            var result = await controller.GetArticle(articleId);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(OkObjectResult));
        }

        [Fact]
        public async void ArticleController_GetProductsByArticle_ReturnOK()
        {
            //Arrange
            var articleId = 1;
            var productList = A.Fake<ICollection<Product>>();
            var productDtoList = A.Fake<List<ProductDto>>();
            A.CallTo(() => _articleRepository.ArticleExistsAsync(articleId)).Returns(true);
            A.CallTo(() => _mapper.Map<List<ProductDto>>(productList)).Returns(productDtoList);
            var controller = new ArticleController(_articleRepository, _mapper, _cacheService, _logger);

            //Act
            var result = await controller.GetArticle(articleId);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(OkObjectResult));
        }

        [Fact]
        public async void ArticleController_CreateArticle_ReturnCreatedAt()
        {
            //Arrange
            var articleCreateDto = A.Fake<ArticleDto>();
            var article = A.Fake<Article>();
            A.CallTo(() => _articleRepository.GetArticleTrimToLowerAsync(articleCreateDto)).Returns(Task.FromResult<Article>(null));
            A.CallTo(() => _mapper.Map<Article>(articleCreateDto)).Returns(article);
            A.CallTo(() => _articleRepository.CreateArticleAsync(article)).Returns(true);
            var controller = new ArticleController(_articleRepository, _mapper, _cacheService, _logger);

            //Act
            var result = await controller.CreateArticle(articleCreateDto);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(CreatedAtActionResult));
        }

        [Fact]
        public async void ArticleController_UpdateArticle_ReturnNoContent()
        {
            //Arrange
            var articleId = 1;
            var articleUpdateDto = A.Fake<ArticleDto>();
            articleUpdateDto.Id = 1;
            var article = A.Fake<Article>();
            A.CallTo(() => _articleRepository.ArticleExistsAsync(articleId)).Returns(true);
            A.CallTo(() => _mapper.Map<Article>(articleUpdateDto)).Returns(article);
            A.CallTo(() => _articleRepository.UpdateArticleAsync(article)).Returns(true);
            var controller = new ArticleController(_articleRepository, _mapper, _cacheService, _logger);

            //Act
            var result = await controller.UpdateArticle(articleId, articleUpdateDto);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(NoContentResult));
        }

        [Fact]
        public async void ArticleController_DeleteArticle_ReturnNoContent()
        {
            //Arrange
            var articleId = 1;
            var article = A.Fake<Article>();
            A.CallTo(() => _articleRepository.ArticleExistsAsync(articleId)).Returns(true);
            A.CallTo(() => _articleRepository.GetArticleAsync(articleId)).Returns(article);
            A.CallTo(() => _articleRepository.DeleteArticleAsync(article)).Returns(true);
            var controller = new ArticleController(_articleRepository, _mapper, _cacheService, _logger);

            //Act
            var result = await controller.DeleteArticle(articleId);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(NoContentResult));
        }

    }
}
