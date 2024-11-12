using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Notino.Dtos;
using Notino.Interfaces;
using Notino.Models;
using System.Diagnostics;

namespace Notino.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductRepository _productRepository;
        private readonly IArticleRepository _articleRepository;
        private readonly IMapper _mapper;
        private readonly ICacheService _cacheService;
        private readonly ILogger<ProductController> _logger;

        public ProductController(
            IProductRepository productRepository, 
            IArticleRepository articleRepository,
            IMapper mapper,
            ICacheService cacheService, 
            ILogger<ProductController> logger)
        {
            _productRepository = productRepository;
            _articleRepository = articleRepository;
            _mapper = mapper;
            _logger = logger;
            _cacheService = cacheService;
        }

        // GET: api/Product
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(ICollection<Product>))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetProducts([FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 5)
        {
            var stopwatch = Stopwatch.StartNew();

            if (pageIndex <= 0 || pageSize <= 0)
                return BadRequest($"{nameof(pageIndex)} and {nameof(pageSize)} size must be greater than 0.");

            // Retrieve from cache/DB
            PagedResponse<Product> products;
            var cacheKey = $"Products_Page_{pageIndex}_Size_{pageSize}";
            if (await _cacheService.ExistsAsync(cacheKey))
            {
                products = await _cacheService.GetAsync<PagedResponse<Product>>(cacheKey);
            }
            else
            {
                products = await _productRepository.GetProductsAsync(pageIndex, pageSize);
                await _cacheService.SetAsync(cacheKey, products, TimeSpan.FromMinutes(1), TimeSpan.FromSeconds(20));
            }
            var productDtos = _mapper.Map<PagedResponseDto<ProductDto>>(products);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            stopwatch.Stop();
            _logger.LogInformation($"Products_Page: {pageIndex}, Size: {pageSize}, Total execution: {stopwatch.ElapsedMilliseconds}ms");

            return Ok(products);
        }

        // GET: api/Product/id
        [HttpGet("{productId}")]
        [ProducesResponseType(200, Type = typeof(Product))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetProduct(int productId)
        {
            var stopwatch = Stopwatch.StartNew();

            // Retrieve from cache/DB
            var cacheKey = $"Product_ {productId}";
            Product product;
            if (await _cacheService.ExistsAsync(cacheKey))
            {
                product = await _cacheService.GetAsync<Product>(cacheKey);
            }
            else
            {
                product = await _productRepository.GetProductAsync(productId);
                await _cacheService.SetAsync(cacheKey, product, TimeSpan.FromMinutes(1), TimeSpan.FromSeconds(20));
            }
            var productDto = _mapper.Map<ProductDto>(product);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (product == null)
            {
                return NotFound();
            }

            stopwatch.Stop();
            _logger.LogInformation($"Product: {productId}, Total execution: {stopwatch.ElapsedMilliseconds}ms");

            return Ok(product);
        }

        // POST: api/Product
        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(422)]
        public async Task<IActionResult> CreateProduct([FromQuery] int articleId, [FromBody] ProductDto productCreateDto)
        {
            if (productCreateDto == null)
            {
                return BadRequest();
            }

            var productFromDb = await _productRepository.GetProductTrimToLowerAsync(productCreateDto);

            if (productFromDb != null)
            {
                ModelState.AddModelError("", "Product already exists!");
                return UnprocessableEntity(ModelState);
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var productCreate = _mapper.Map<Product>((productCreateDto));
            productCreate.Article = await _articleRepository.GetArticleAsync(articleId);

            if (!await _productRepository.CreateProductAsync(productCreate))
            {
                ModelState.AddModelError("", "Something went wrong while saving..");
                return StatusCode(500, ModelState);
            }

            return CreatedAtAction("CreateProduct",
                new { id = productCreate.Id },
                _mapper.Map<ProductDto>(productCreate));
        }

        // PUT: api/Product
        [HttpPut("{productId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateProduct(int productId, [FromBody] ProductDto productUpdateDto)
        {
            if (productUpdateDto == null)
            {
                return BadRequest(ModelState);
            }
            if (productId != productUpdateDto.Id)
            {
                return BadRequest(ModelState);
            }
            if (!await _productRepository.ProductExistsAsync(productId))
            {
                return NotFound();
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var product = _mapper.Map<Product>(productUpdateDto);

            if (!await _productRepository.UpdateProductAsync(product))
            {
                ModelState.AddModelError("", "Something went wrong while saving..");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }

        // DELETE: api/Product/id
        [HttpDelete("{productId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteProduct(int productId)
        {
            if (!await _productRepository.ProductExistsAsync(productId))
            {
                return NotFound();
            }

            var article = await _productRepository.GetProductAsync(productId);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!await _productRepository.DeleteProductAsync(article))
            {
                ModelState.AddModelError("", "Something went wrong while saving..");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }
    }
}
