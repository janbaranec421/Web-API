using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Notino.Dtos;
using Notino.Interfaces;
using Notino.Models;

namespace Notino.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductRepository _productRepository;
        private readonly IArticleRepository _articleRepository;
        private readonly IMapper _mapper;

        public ProductController(IProductRepository productRepository, IArticleRepository articleRepository, IMapper mapper)
        {
            _productRepository = productRepository;
            _articleRepository = articleRepository;
            _mapper = mapper;
        }

        // GET: api/Product
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(ICollection<Product>))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetProducts()
        {
            var products = _mapper.Map<ICollection<ProductDto>>(await _productRepository.GetProducts());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok(products);
        }

        // GET: api/Product/id
        [HttpGet("{productId}")]
        [ProducesResponseType(200, Type = typeof(Product))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetProduct(int productId)
        {
            var product = _mapper.Map<ProductDto>(await _productRepository.GetProduct(productId));

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (product == null)
            {
                return NotFound();
            }
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

            var productFromDb = await _productRepository.GetProductTrimToLower(productCreateDto);

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
            productCreate.Article = await _articleRepository.GetArticle(articleId);

            if (!await _productRepository.CreateProduct(productCreate))
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
            if (!await _productRepository.ProductExists(productId))
            {
                return NotFound();
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var product = _mapper.Map<Product>(productUpdateDto);

            if (!await _productRepository.UpdateProduct(product))
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
            if (!await _productRepository.ProductExists(productId))
            {
                return NotFound();
            }

            var article = await _productRepository.GetProduct(productId);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!await _productRepository.DeleteProduct(article))
            {
                ModelState.AddModelError("", "Something went wrong while saving..");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }
    }
}
