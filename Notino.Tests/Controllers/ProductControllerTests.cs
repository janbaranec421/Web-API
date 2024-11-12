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
    public class ProductControllerTests
    {
        private readonly IProductRepository _productRepository;
        private readonly IArticleRepository _articleRepository;
        private readonly IMapper _mapper;
        private readonly ICacheService _cacheService;
        private readonly ILogger<ProductController> _logger;

        public ProductControllerTests()
        {
            _productRepository = A.Fake<IProductRepository>();
            _articleRepository = A.Fake<IArticleRepository>();
            _mapper = A.Fake<IMapper>();
            _cacheService = A.Fake<ICacheService>();
            _logger = A.Fake<ILogger<ProductController>>();
        }

        [Fact]
        public async void ProductController_GetProducts_ReturnOK()
        {
            //Arrange
            var products = A.Fake<ICollection<ProductDto>>();
            var productsList = A.Fake<List<ProductDto>>();
            A.CallTo(() => _mapper.Map<ICollection<ProductDto>>(products)).Returns(productsList);
            var controller = new ProductController(_productRepository, _articleRepository, _mapper, _cacheService, _logger);

            //Act
            var result = await controller.GetProducts();

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(OkObjectResult));
        }

        [Fact]
        public async void ProductController_GetProduct_ReturnOK()
        {
            //Arrange
            var productId = 1;
            var product = A.Fake<Product>();
            var productDto = A.Fake<ProductDto>();
            A.CallTo(() => _mapper.Map<ProductDto>(product)).Returns(productDto);
            var controller = new ProductController(_productRepository, _articleRepository, _mapper, _cacheService, _logger);

            //Act
            var result = await controller.GetProduct(productId);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(OkObjectResult));
        }

        [Fact]
        public async void ProductController_CreateProduct_ReturnCreatedAt()
        {
            //Arrange
            var articleId = 1;
            var productCreateDto = A.Fake<ProductDto>();
            var product = A.Fake<Product>();
            A.CallTo(() => _productRepository.GetProductTrimToLowerAsync(productCreateDto)).Returns(Task.FromResult<Product>(null));
            A.CallTo(() => _mapper.Map<Product>(productCreateDto)).Returns(product);
            A.CallTo(() => _productRepository.CreateProductAsync(product)).Returns(true);
            var controller = new ProductController(_productRepository, _articleRepository, _mapper, _cacheService, _logger);

            //Act
            var result = await controller.CreateProduct(articleId, productCreateDto);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(CreatedAtActionResult));
        }

        [Fact]
        public async void ProductController_UpdateProduct_ReturnNoContent()
        {
            //Arrange
            var productId = 1;
            var productUpdateDto = A.Fake<ProductDto>();
            productUpdateDto.Id = 1;
            var product = A.Fake<Product>();
            A.CallTo(() => _productRepository.ProductExistsAsync(productId)).Returns(true);
            A.CallTo(() => _mapper.Map<Product>(productUpdateDto)).Returns(product);
            A.CallTo(() => _productRepository.UpdateProductAsync(product)).Returns(true);
            var controller = new ProductController(_productRepository, _articleRepository, _mapper, _cacheService, _logger);

            //Act
            var result = await controller.UpdateProduct(productId, productUpdateDto);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(NoContentResult));
        }

        [Fact]
        public async void ProductController_DeleteProduct_ReturnNoContent()
        {
            //Arrange
            var productId = 1;
            var product = A.Fake<Product>();
            A.CallTo(() => _productRepository.ProductExistsAsync(productId)).Returns(true);
            A.CallTo(() => _productRepository.GetProductAsync(productId)).Returns(product);
            A.CallTo(() => _productRepository.DeleteProductAsync(product)).Returns(true);
            var controller = new ProductController(_productRepository, _articleRepository, _mapper, _cacheService, _logger);

            //Act
            var result = await controller.DeleteProduct(productId);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(NoContentResult));
        }

    }
}
