using System.ComponentModel.DataAnnotations;

namespace Blog.ViewModels;

public class CommentCreateViewModel
{
    [Required]
    public string Content { get; set; }

    [Required]
    public int ArticleId { get; set; }

    // UserId делаем nullable, чтобы можно было создавать комментарии без автора
    public int? UserId { get; set; }
}
