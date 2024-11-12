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
    public class ArticleController : ControllerBase
    {
        private readonly IArticleRepository _articleRepository;
        private readonly IMapper _mapper;
        private readonly ICacheService _cacheService;
        private readonly ILogger<ArticleController> _logger;

        public ArticleController(
            IArticleRepository articleRepository,
            IMapper mapper,
            ICacheService cacheService,
            ILogger<ArticleController> logger)
        {
            _articleRepository = articleRepository;
            _mapper = mapper;
            _cacheService = cacheService;
            _logger = logger;
        }

        // GET: api/Article
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(ICollection<Article>))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetArticles([FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 5)
        {
            var stopwatch = Stopwatch.StartNew();

            if (pageIndex <= 0 || pageSize <= 0)
                return BadRequest($"{nameof(pageIndex)} and {nameof(pageSize)} size must be greater than 0.");

            // Retrieve from cache/DB
            PagedResponse<Article> articles;
            var cacheKey = $"Articles_Page_{pageIndex}_Size_{pageSize}";
            if (await _cacheService.ExistsAsync(cacheKey))
            {
                articles = await _cacheService.GetAsync<PagedResponse<Article>>(cacheKey);
            }
            else
            {
                articles = await _articleRepository.GetArticlesAsync(pageIndex, pageSize);
                await _cacheService.SetAsync(cacheKey, articles, TimeSpan.FromMinutes(1), TimeSpan.FromSeconds(20));
            }
            var articleDtos = _mapper.Map<PagedResponseDto<ArticleDto>>(articles);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            stopwatch.Stop();
            _logger.LogInformation($"Articles_Page: {pageIndex}, Size: {pageSize}, Total execution: {stopwatch.ElapsedMilliseconds}ms");

            return Ok(articleDtos);
        }

        // GET: api/Article/id
        [HttpGet("{articleId}")]
        [ProducesResponseType(200, Type = typeof(Article))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetArticle(int articleId)
        {
            var stopwatch = Stopwatch.StartNew();

            // Retrieve from cache/DB
            var cacheKey = $"Article_ {articleId}";
            Article article;
            if (await _cacheService.ExistsAsync(cacheKey))
            {
                article = await _cacheService.GetAsync<Article>(cacheKey);
            }
            else
            {
                article = await _articleRepository.GetArticleAsync(articleId);
                await _cacheService.SetAsync(cacheKey, article, TimeSpan.FromMinutes(1), TimeSpan.FromSeconds(20));
            }
            var articleDto = _mapper.Map<ArticleDto>(article);
            
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (articleDto == null)
            {
                return NotFound();
            }

            stopwatch.Stop();
            _logger.LogInformation($"Article: {articleId}, Total execution: {stopwatch.ElapsedMilliseconds}ms");

            return Ok(articleDto);
        }

        // GET: api/Article/id/products
        [HttpGet("{articleId}/products")]
        [ProducesResponseType(200, Type = typeof(ICollection<Product>))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetProductsByArticle(int articleId)
        {
            if (!await _articleRepository.ArticleExistsAsync(articleId))
            {
                return NotFound();
            }

            var products = _mapper.Map<List<ProductDto>>(await _articleRepository.GetProductsByArticleAsync(articleId));

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

            var articleFromDb = await _articleRepository.GetArticleTrimToLowerAsync(articleCreateDto);

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

            if (!await _articleRepository.CreateArticleAsync(articleCreate))
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
            if (!await _articleRepository.ArticleExistsAsync(articleId))
            {
                return NotFound();
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var article = _mapper.Map<Article>(articleUpdateDto);

            if (!await _articleRepository.UpdateArticleAsync(article))
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
            if (!await _articleRepository.ArticleExistsAsync(articleId))
            {
                return NotFound();
            }

            var article = await _articleRepository.GetArticleAsync(articleId);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!await _articleRepository.DeleteArticleAsync(article))
            {
                ModelState.AddModelError("", "Something went wrong while saving..");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }
    }
}
