using Blog.Models;
using System.Collections.Generic;

namespace Blog.ViewModels;

/// <summary>
/// Модель для страницы со списком статей.
/// </summary>
public class ArticlesIndexViewModel
{
    /// <summary>
    /// Коллекция статей для отображения.
    /// </summary>
    public IEnumerable<Article> Articles { get; set; } = new List<Article>();
}