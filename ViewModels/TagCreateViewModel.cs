using System.ComponentModel.DataAnnotations;

namespace Blog.ViewModels;

public class TagCreateViewModel
{
    [Required]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "Имя тега должно быть от 2 до 50 символов")]
    public string Name { get; set; }
}
