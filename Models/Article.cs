using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Blog.Models;

public class Article
{
    public int Id { get; set; }

    [Required]
    [StringLength(200)]
    public string Title { get; set; }

    [Required]
    public string Content { get; set; }

    // Внешний ключ на пользователя-автора
    public int AuthorId { get; set; }

    // Навигационное свойство
    public User Author { get; set; }

    // Связь с тегами
    public ICollection<ArticleTag> ArticleTags { get; set; } = new List<ArticleTag>();

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}