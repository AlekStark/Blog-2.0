using Blog.Data;
using Blog.Models;
using Microsoft.EntityFrameworkCore;

namespace Blog.Data;

public class BlogContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Article> Articles { get; set; }
    public DbSet<Tag> Tags { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<UserRole> UserRoles { get; set; }
    public DbSet<ArticleTag> ArticleTags { get; set; }

    public BlogContext(DbContextOptions<BlogContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Связь User — Role через UserRole
        modelBuilder.Entity<UserRole>()
            .HasKey(ur => new { ur.UserId, ur.RoleId });

        modelBuilder.Entity<UserRole>()
            .HasOne(ur => ur.User)
            .WithMany(u => u.UserRoles)
            .HasForeignKey(ur => ur.UserId);

        modelBuilder.Entity<UserRole>()
            .HasOne(ur => ur.Role)
            .WithMany(r => r.UserRoles)
            .HasForeignKey(ur => ur.RoleId);

        // Связь Article — Tag через ArticleTag
        modelBuilder.Entity<ArticleTag>()
            .HasKey(at => new { at.ArticleId, at.TagId }); // Составной ключ

        modelBuilder.Entity<ArticleTag>()
            .HasOne(at => at.Article)
            .WithMany(a => a.ArticleTags)
            .HasForeignKey(at => at.ArticleId);

        modelBuilder.Entity<ArticleTag>()
            .HasOne(at => at.Tag)
            .WithMany(t => t.ArticleTags)
            .HasForeignKey(at => at.TagId);

        // Связь Article — User (Author)
        modelBuilder.Entity<Article>()
            .HasOne(a => a.Author)
            .WithMany(u => u.Articles)
            .HasForeignKey(a => a.AuthorId);
    }
}