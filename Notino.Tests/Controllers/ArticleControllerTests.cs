using AutoMapper;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
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

        public ArticleControllerTests()
        {
            _articleRepository = A.Fake<IArticleRepository>();
            _mapper = A.Fake<IMapper>();
        }

        [Fact]
        public async void ArticleController_GetArticles_ReturnOK()
        {
            //Arrange
            var articles = A.Fake<ICollection<ArticleDto>>();
            var articlesList = A.Fake<List<ArticleDto>>();
            A.CallTo(() => _mapper.Map<ICollection<ArticleDto>>(articles)).Returns(articlesList);
            var controller = new ArticleController(_articleRepository, _mapper);

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
            var controller = new ArticleController(_articleRepository, _mapper);

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
            A.CallTo(() => _articleRepository.ArticleExists(articleId)).Returns(true);
            A.CallTo(() => _mapper.Map<List<ProductDto>>(productList)).Returns(productDtoList);
            var controller = new ArticleController(_articleRepository, _mapper);

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
            A.CallTo(() => _articleRepository.GetArticleTrimToLower(articleCreateDto)).Returns(Task.FromResult<Article>(null));
            A.CallTo(() => _mapper.Map<Article>(articleCreateDto)).Returns(article);
            A.CallTo(() => _articleRepository.CreateArticle(article)).Returns(true);
            var controller = new ArticleController(_articleRepository, _mapper);

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
            A.CallTo(() => _articleRepository.ArticleExists(articleId)).Returns(true);
            A.CallTo(() => _mapper.Map<Article>(articleUpdateDto)).Returns(article);
            A.CallTo(() => _articleRepository.UpdateArticle(article)).Returns(true);
            var controller = new ArticleController(_articleRepository, _mapper);

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
            A.CallTo(() => _articleRepository.ArticleExists(articleId)).Returns(true);
            A.CallTo(() => _articleRepository.GetArticle(articleId)).Returns(article);
            A.CallTo(() => _articleRepository.DeleteArticle(article)).Returns(true);
            var controller = new ArticleController(_articleRepository, _mapper);

            //Act
            var result = await controller.DeleteArticle(articleId);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(NoContentResult));
        }

    }
}
