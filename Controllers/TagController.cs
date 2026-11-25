using Blog.Data;
using Blog.Models;
using Blog.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Blog.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TagController : Controller
{
    private readonly BlogContext _context;
    public TagController(BlogContext context)
    {
        _context = context;
    }

    // POST: api/Tags
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] TagCreateViewModel model)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        // Проверяем, что тег с таким именем ещё не существует
        var existingTag = await _context.Tags.FirstOrDefaultAsync(t => t.Name.ToLower() == model.Name.ToLower());
        if (existingTag != null)
        {
            ModelState.AddModelError("Name", "Тег с таким именем уже существует");
            return BadRequest(ModelState);
        }

        var tag = new Tag
        {
            Name = model.Name,
            CreatedAt = DateTime.UtcNow
        };

        _context.Tags.Add(tag);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Тег успешно создан", tagId = tag.Id });
    }

    // PUT: api/Tag/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> Edit(int id, [FromBody] TagEditViewModel model)
    {
        if (id != model.Id)
        {
            return BadRequest("ID тега в URL и теле запроса не совпадают");
        }

        if (!ModelState.IsValid) return BadRequest(ModelState);

        var tag = await _context.Tags.FindAsync(id);
        if (tag == null)
        {
            return NotFound("Тег не найден");
        }

        // Проверяем, что тег с таким именем ещё не существует (кроме текущего)
        var existingTag = await _context.Tags.FirstOrDefaultAsync(t => t.Name.ToLower() == model.Name.ToLower() && t.Id != id);
        if (existingTag != null)
        {
            ModelState.AddModelError("Name", "Тег с таким именем уже существует");
            return BadRequest(ModelState);
        }

        tag.Name = model.Name;
        tag.UpdatedAt = DateTime.UtcNow;

        _context.Tags.Update(tag);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Тег успешно обновлён", tagId = tag.Id });
    }

    // DELETE: api/Tag/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var tag = await _context.Tags.FindAsync(id);
        if (tag == null)
        {
            return NotFound("Тег не найден");
        }

        // Проверяем, что тег не используется в статьях
        var isUsed = await _context.ArticleTags.AnyAsync(at => at.TagId == id);
        if (isUsed)
        {
            return BadRequest("Тег используется в статьях и не может быть удалён");
        }

        _context.Tags.Remove(tag);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Тег успешно удалён", tagId = id });
    }

    // GET: api/Tags
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var tags = await _context.Tags
            .Select(t => new
            {
                t.Id,
                t.Name,
                t.CreatedAt,
                t.UpdatedAt,
                ArticlesCount = t.ArticleTags.Count
            })
            .ToListAsync();

        return Ok(tags);
    }

    // GET: api/Tags/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var tag = await _context.Tags
            .Where(t => t.Id == id)
            .Select(t => new
            {
                t.Id,
                t.Name,
                t.CreatedAt,
                t.UpdatedAt,
                ArticlesCount = t.ArticleTags.Count
            })
            .FirstOrDefaultAsync();

        if (tag == null)
        {
            return NotFound("Тег не найден");
        }

        return Ok(tag);
    }
}