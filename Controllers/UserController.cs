using Blog.Data;
using Blog.Filters;
using Blog.Models;
using Blog.Utils;
using Blog.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Blog.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly BlogContext _context;
    public UsersController(BlogContext context)
    {
        _context = context;
    }

    // POST: api/Users/register
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterViewModel model)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        if (await _context.Users.AnyAsync(u => u.Username == model.Username))
        {
            ModelState.AddModelError("Username", "Пользователь с таким логином уже существует");
            return BadRequest(ModelState);
        }

        if (await _context.Users.AnyAsync(u => u.Email == model.Email))
        {
            ModelState.AddModelError("Email", "Пользователь с таким email уже существует");
            return BadRequest(ModelState);
        }

        // Хешируем пароль
        var (salt, passwordHash) = PasswordHasher.HashPassword(model.Password);

        var user = new User
        {
            Username = model.Username,
            Email = model.Email,
            PasswordHash = passwordHash, // Сохраняем хеш, а не обычный пароль!
            CreatedAt = DateTime.UtcNow,
            UserRoles = new List<UserRole>() // Инициализируем коллекцию
        };

        // Присваиваем роль "Пользователь" по умолчанию
        var defaultRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "Пользователь");
        if (defaultRole != null)
        {
            user.UserRoles.Add(new UserRole { Role = defaultRole });
        }

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Пользователь успешно создан", userId = user.Id, role = defaultRole?.Name });
    }

    // PUT: api/User/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> Edit(int id, [FromBody] EditUserViewModel model)
    {
        if (id != model.Id)
        {
            return BadRequest("ID пользователя в URL и теле запроса не совпадают");
        }

        if (!ModelState.IsValid) return BadRequest(ModelState);

        var user = await _context.Users
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Id == id);

        if (user == null)
        {
            return NotFound("Пользователь не найден");
        }

        // Проверяем уникальность логина и email (кроме текущего пользователя)
        if (await _context.Users.AnyAsync(u => u.Username == model.Username && u.Id != id))
        {
            ModelState.AddModelError("Username", "Пользователь с таким логином уже существует");
            return BadRequest(ModelState);
        }

        if (await _context.Users.AnyAsync(u => u.Email == model.Email && u.Id != id))
        {
            ModelState.AddModelError("Email", "Пользователь с таким email уже существует");
            return BadRequest(ModelState);
        }

        // Обновляем данные пользователя
        user.Username = model.Username;
        user.Email = model.Email;

        // Если пришёл новый пароль — хешируем и сохраняем
        if (!string.IsNullOrEmpty(model.Password))
        {
            var (_, passwordHash) = PasswordHasher.HashPassword(model.Password);
            user.PasswordHash = passwordHash;
        }

        // Обновляем роль, если она изменилась
        if (!string.IsNullOrEmpty(model.Role))
        {
            var newRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == model.Role);
            if (newRole != null)
            {
                // Удаляем старые роли
                user.UserRoles.Clear();
                // Добавляем новую роль
                user.UserRoles.Add(new UserRole { Role = newRole });
            }
        }

        _context.Users.Update(user);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Пользователь успешно обновлён", userId = user.Id });
    }

    // DELETE: api/Users/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
        {
            return NotFound("Пользователь не найден");
        }

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Пользователь успешно удалён", userId = id });
    }


    /// <summary>
    /// фильтр только для админов.
    /// при авторизации получить токен
    /// Key: Authorization
    ///Value: Bearer<ваш_токен>
    /// </summary>
    /// <returns></returns>
    // GET: api/Users
    [HttpGet]
    [ServiceFilter(typeof(AdminOnlyFilter))] // 
    public async Task<IActionResult> GetAll()
        {
            var users = await _context.Users
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .Select(u => new
                {
                    u.Id,
                    u.Username,
                    u.Email,
                    u.CreatedAt,
                    Roles = u.UserRoles.Select(ur => ur.Role.Name).ToList()
                })
                .ToListAsync();

            return Ok(users);
        }

        // GET: api/Users/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var user = await _context.Users
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .Where(u => u.Id == id)
                .Select(u => new
                {
                    u.Id,
                    u.Username,
                    u.Email,
                    u.CreatedAt,
                    Roles = u.UserRoles.Select(ur => ur.Role.Name).ToList()
                })
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return NotFound("Пользователь не найден");
            }

            return Ok(user);
        }
}