using Blog.Data;
using Blog.Models;
using Blog.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Blog.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CommentController : Controller
{
    private readonly BlogContext _context;
    public CommentController(BlogContext context)
    {
        _context = context;
    }
    // POST: api/Comment
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CommentCreateViewModel model)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        // Проверяем существование статьи
        var article = await _context.Articles.FindAsync(model.ArticleId);
        if (article == null)
        {
            ModelState.AddModelError("ArticleId", "Статья не найдена");
            return BadRequest(ModelState);
        }

        // Проверяем существование автора (если userId указан)
        if (model.UserId.HasValue)
        {
            var user = await _context.Users.FindAsync(model.UserId.Value);
            if (user == null)
            {
                ModelState.AddModelError("UserId", "Автор не найден");
                return BadRequest(ModelState);
            }
        }

        var comment = new Comment
        {
            Content = model.Content,
            ArticleId = model.ArticleId,
            UserId = (int)model.UserId,
            CreatedAt = DateTime.UtcNow
        };

        _context.Comments.Add(comment);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Комментарий успешно создан", commentId = comment.Id });
    }
    // PUT: api/Comment/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> Edit(int id, [FromBody] CommentEditViewModel model)
    {
        if (id != model.Id)
        {
            return BadRequest("ID комментария в URL и теле запроса не совпадают");
        }

        if (!ModelState.IsValid) return BadRequest(ModelState);

        var comment = await _context.Comments.FindAsync(id);
        if (comment == null)
        {
            return NotFound("Комментарий не найден");
        }

        // Проверяем существование статьи
        var article = await _context.Articles.FindAsync(model.ArticleId);
        if (article == null)
        {
            ModelState.AddModelError("ArticleId", "Статья не найдена");
            return BadRequest(ModelState);
        }

        // Проверяем существование автора (если userId указан)
        if (model.UserId.HasValue)
        {
            var user = await _context.Users.FindAsync(model.UserId.Value);
            if (user == null)
            {
                ModelState.AddModelError("UserId", "Автор не найден");
                return BadRequest(ModelState);
            }
        }

        // Обновляем поля
        comment.Content = model.Content;
        comment.ArticleId = model.ArticleId;
        comment.UserId = (int)model.UserId;
        comment.UpdatedAt = DateTime.UtcNow;

        _context.Comments.Update(comment);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Комментарий успешно обновлён", commentId = comment.Id });
    }

    // DELETE: api/Comment/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var comment = await _context.Comments.FindAsync(id);
        if (comment == null)
        {
            return NotFound("Комментарий не найден");
        }

        _context.Comments.Remove(comment);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Комментарий успешно удалён", commentId = id });
    }
    // GET: api/Comments
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var comments = await _context.Comments
            .Include(c => c.Article)
            .Include(c => c.User)
            .Select(c => new
            {
                c.Id,
                c.Content,
                c.ArticleId,
                c.UserId,
                c.CreatedAt,
                c.UpdatedAt,
                Article = c.Article != null ? new { c.Article.Id, c.Article.Title } : null,
                User = c.User != null ? new { c.User.Id, c.User.Username } : null
            })
            .ToListAsync();

        return Ok(comments);
    }

    // GET: api/Comments/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var comment = await _context.Comments
            .Include(c => c.Article)
            .Include(c => c.User)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (comment == null)
        {
            return NotFound("Комментарий не найден");
        }

        return Ok(new
        {
            comment.Id,
            comment.Content,
            comment.ArticleId,
            comment.UserId,
            comment.CreatedAt,
            comment.UpdatedAt,
            Article = comment.Article != null ? new { comment.Article.Id, comment.Article.Title } : null,
            User = comment.User != null ? new { comment.User.Id, comment.User.Username } : null
        });
    }
}
