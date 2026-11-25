using System.ComponentModel.DataAnnotations;


namespace Blog.ViewModels;
    public class EditUserViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Логин обязателен")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Логин должен быть от 3 до 50 символов")]
        [Display(Name = "Логин")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Email обязателен")]
        [EmailAddress(ErrorMessage = "Некорректный формат email")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [StringLength(100, MinimumLength = 6, ErrorMessage = "Пароль должен быть от 6 до 100 символов")]
        [DataType(DataType.Password)]
        [Display(Name = "Новый пароль (опционально)")]
        public string Password { get; set; }

        [Display(Name = "Роль (опционально)")]
        public string Role { get; set; }
    }

