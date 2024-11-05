using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Notino.Dtos;
using Notino.Interfaces;
using Notino.Models;

namespace Notino.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArticleController : ControllerBase
    {
        private readonly IArticleRepository _articleRepository;
        private readonly IMapper _mapper;

        public ArticleController(IArticleRepository articleRepository, IMapper mapper)
        {
            _articleRepository = articleRepository;
            _mapper = mapper;
        }

        // GET: api/Article
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(ICollection<Article>))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetArticles([FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 5)
        {
            if (pageIndex <= 0 || pageSize <= 0)
                return BadRequest($"{nameof(pageIndex)} and {nameof(pageSize)} size must be greater than 0.");

            var articles = _mapper.Map<PagedResponseDto<ArticleDto>>(await _articleRepository.GetArticles(pageIndex, pageSize));

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok(articles);
        }

        // GET: api/Article/id
        [HttpGet("{articleId}")]
        [ProducesResponseType(200, Type = typeof(Article))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetArticle(int articleId)
        {
            var article = _mapper.Map<ArticleDto>(await _articleRepository.GetArticle(articleId));

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (article == null)
            {
                return NotFound();
            }
            return Ok(article);
        }

        // GET: api/Article/id/products
        [HttpGet("{articleId}/products")]
        [ProducesResponseType(200, Type = typeof(ICollection<Product>))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetProductsByArticle(int articleId)
        {
            if (!await _articleRepository.ArticleExists(articleId))
            {
                return NotFound();
            }

            var products = _mapper.Map<List<ProductDto>>(await _articleRepository.GetProductsByArticle(articleId));

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (products == null)
            {
                return NotFound();
            }
            return Ok(products);
        }

        // POST: api/Article
        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(422)]
        public async Task<IActionResult> CreateArticle([FromBody] ArticleDto articleCreateDto)
        {
            if (articleCreateDto == null)
            {
                return BadRequest();
            }

            var articleFromDb = await _articleRepository.GetArticleTrimToLower(articleCreateDto);

            if (articleFromDb != null)
            {
                ModelState.AddModelError("", "Article already exists!");
                return UnprocessableEntity(ModelState);
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var articleCreate = _mapper.Map<Article>((articleCreateDto));

            if (!await _articleRepository.CreateArticle(articleCreate))
            {
                ModelState.AddModelError("", "Something went wrong while saving..");
                return StatusCode(500, ModelState);
            }

            return CreatedAtAction("CreateArticle", 
                new { id = articleCreate.Id}, 
                _mapper.Map<ArticleDto>(articleCreate));
        }

        // PUT: api/Article
        [HttpPut("{articleId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateArticle(int articleId, [FromBody] ArticleDto articleUpdateDto)
        {
            if (articleUpdateDto == null)
            {
                return BadRequest(ModelState);
            }
            if (articleId != articleUpdateDto.Id)
            {
                return BadRequest(ModelState);
            }
            if (!await _articleRepository.ArticleExists(articleId))
            {
                return NotFound();
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var article = _mapper.Map<Article>(articleUpdateDto);

            if (!await _articleRepository.UpdateArticle(article))
            {
                ModelState.AddModelError("", "Something went wrong while saving..");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }

        // DELETE: api/Article/id
        [HttpDelete("{articleId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteArticle(int articleId)
        {
            if (!await _articleRepository.ArticleExists(articleId))
            {
                return NotFound();
            }

            var article = await _articleRepository.GetArticle(articleId);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!await _articleRepository.DeleteArticle(article))
            {
                ModelState.AddModelError("", "Something went wrong while saving..");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }
    }
}
