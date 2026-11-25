using System.ComponentModel.DataAnnotations;

namespace Blog.ViewModels;

public class CommentEditViewModel
{
    public int Id { get; set; }

    [Required]
    public string Content { get; set; }

    [Required]
    public int ArticleId { get; set; }

    // UserId делаем nullable, чтобы можно было снимать автора или назначать нового
    public int? UserId { get; set; }
}
