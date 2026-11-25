using Blog.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Blog.Models;

public class UserRole
{
    [Key]
    public int UserId { get; set; }
    public User User { get; set; }

    [Key]
    public int RoleId { get; set; }
    public Role Role { get; set; }
}