namespace Blog.Models;

public class Tag
{
    public int Id { get; set; }
    public string Name { get; set; }
    public ICollection<ArticleTag> ArticleTags { get; set; } = new List<ArticleTag>();
    public DateTime CreatedAt { get; internal set; }
    public DateTime UpdatedAt { get; internal set; }
}
