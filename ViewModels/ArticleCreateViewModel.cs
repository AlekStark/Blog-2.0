using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace Blog.ViewModels;

public class ArticleCreateViewModel
{
    [Required(ErrorMessage = "Заголовок обязателен")]
    [StringLength(200, MinimumLength = 3, ErrorMessage = "Заголовок должен быть от 3 до 200 символов")]
    [Display(Name = "Заголовок")]
    public string Title { get; set; }

    [Required(ErrorMessage = "Текст статьи обязателен")]
    [Display(Name = "Текст статьи")]
    public string Content { get; set; }

    [Required(ErrorMessage = "Id автора обязателен")]
    [Display(Name = "Id автора")]
    public int AuthorId { get; set; }

    [Display(Name = "Теги (id через запятую, опционально)")] 
    public List<int>? TagIds { get; set; }
}