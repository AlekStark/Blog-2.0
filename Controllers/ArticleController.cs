using Blog.Data;
using Blog.Filters;
using Blog.Models;
using Blog.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.Controllers;

//[ApiController]
//[Route("api/[controller]")]
public class ArticlesController : ControllerBase
{
    private readonly BlogContext _context;
    public ArticlesController(BlogContext context)
    {
        _context = context;
    }

    // POST: api/Article
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ArticleCreateViewModel model)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        // Проверяем существование автора
        var author = await _context.Users.FindAsync(model.AuthorId);
        if (author == null)
        {
            ModelState.AddModelError("AuthorId", "Автор не найден");
            return BadRequest(ModelState);
        }

        // Обработка тегов: если TagIds не null и не пуст — ищем теги, иначе создаём статью без тегов
        List<Tag> tags = new List<Tag>();
        if (model.TagIds != null && model.TagIds.Any())
        {
            tags = await _context.Tags
                .Where(t => model.TagIds.Contains(t.Id))
                .ToListAsync();

            // Проверяем, что все указанные теги найдены
            if (tags.Count != model.TagIds.Distinct().Count())
            {
                ModelState.AddModelError("TagIds", "Один или несколько тегов не найдены");
                return BadRequest(ModelState);
            }
        }

        var article = new Article
        {
            Title = model.Title,
            Content = model.Content,
            AuthorId = model.AuthorId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            ArticleTags = tags.Select(t => new ArticleTag { Tag = t }).ToList()
        };

        _context.Articles.Add(article);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Статья успешно создана", articleId = article.Id });
    }

    // PUT: api/Article/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> Edit(int id, [FromBody] ArticleEditViewModel model)
    {
        if (id != model.Id)
        {
            return BadRequest("ID статьи в URL и теле запроса не совпадают");
        }

        if (!ModelState.IsValid) return BadRequest(ModelState);

        var article = await _context.Articles.FindAsync(id);

        if (article == null)
        {
            return NotFound("Статья не найдена");
        }

        // Обновляем только заголовок и содержимое
        article.Title = model.Title;
        article.Content = model.Content;
        article.UpdatedAt = DateTime.UtcNow;

        _context.Articles.Update(article);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Статья успешно обновлена", articleId = article.Id });
    }

    // DELETE: api/Article/{id}
    [HttpDelete("{id}")]
    //[ServiceFilter(typeof(AdminOnlyFilter))]  
    public async Task<IActionResult> Delete(int id)
    {
        var article = await _context.Articles.FindAsync(id);
        if (article == null)
        {
            return NotFound("Статья не найдена");
        }

        _context.Articles.Remove(article);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Статья успешно удалена", articleId = id });
    }

    // GET: api/Article
    [HttpGet]
    //[ServiceFilter(typeof(AdminOnlyFilter))]
    public async Task<IActionResult> GetAll()
    {
        var articles = await _context.Articles
            .Include(a => a.Author)
            .Include(a => a.ArticleTags)
            .ThenInclude(at => at.Tag)
            .Select(a => new
            {
                a.Id,
                a.Title,
                a.Content,
                Author = a.Author != null ? new
                {
                    a.Author.Id,
                    a.Author.Username,
                    a.Author.Email
                } : null,
                Tags = a.ArticleTags.Select(at => new
                {
                    at.Tag.Id,
                    at.Tag.Name
                }).ToList(),
                a.CreatedAt,
                a.UpdatedAt
            })
            .ToListAsync();

        return Ok(articles);
    }

    // GET: api/Article/author/{authorId}
    [HttpGet("author/{authorId}")]
    public async Task<IActionResult> GetByAuthor(int authorId)
    {
        var authorExists = await _context.Users.AnyAsync(u => u.Id == authorId);
        if (!authorExists)
        {
            return NotFound("Автор не найден");
        }

        var articles = await _context.Articles
            .Where(a => a.AuthorId == authorId)
            .Include(a => a.Author)
            .Include(a => a.ArticleTags)
            .ThenInclude(at => at.Tag)
            .Select(a => new
            {
                a.Id,
                a.Title,
                a.Content,
                Author = a.Author != null ? new
                {
                    a.Author.Id,
                    a.Author.Username,
                    a.Author.Email
                } : null,
                Tags = a.ArticleTags.Select(at => new
                {
                    at.Tag.Id,
                    at.Tag.Name
                }).ToList(),
                a.CreatedAt,
                a.UpdatedAt
            })
            .ToListAsync();

        return Ok(articles);
    }
}