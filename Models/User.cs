namespace Blog.Models;

public class User
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public ICollection<UserRole> UserRoles { get; set; }
    public ICollection<Article> Articles { get; set; } = new List<Article>();
}