using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace Blog.ViewModels;

public class ArticleEditViewModel
{
    [Required(ErrorMessage = "ID статьи обязателен")]
    public int Id { get; set; }

    [Required(ErrorMessage = "Заголовок обязателен")]
    [StringLength(200, MinimumLength = 3, ErrorMessage = "Заголовок должен быть от 3 до 200 символов")]
    [Display(Name = "Заголовок")]
    public string Title { get; set; }

    [Required(ErrorMessage = "Текст статьи обязателен")]
    [Display(Name = "Текст статьи")]
    public string Content { get; set; }

    // UserId делаем nullable, чтобы можно было снимать автора или назначать нового
    [Display(Name = "Id автора (опционально)")]
    public int? AuthorId { get; set; }

    // Теги делаем необязательными: nullable List<int>
    [Display(Name = "Теги (id через запятую, опционально)")]
    public List<int>? TagIds { get; set; }
}
