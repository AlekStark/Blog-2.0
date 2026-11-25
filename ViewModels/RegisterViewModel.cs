using System.ComponentModel.DataAnnotations;

namespace Blog.ViewModels;

// Модель для регистрации
public class RegisterViewModel
{
    [Required]
    [Display(Name = "Логин")]
    public string Username { get; set; }

    [Required]
    [EmailAddress]
    [Display(Name = "Email")]
    public string Email { get; set; }

    [Required]
    [DataType(DataType.Password)]
    [Display(Name = "Пароль")]
    public string Password { get; set; }

    [Display(Name = "Роль")]
    public string Role { get; set; }
}
