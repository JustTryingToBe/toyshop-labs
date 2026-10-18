using Microsoft.AspNetCore.Mvc.RazorPages;
using ToyShop.Core;

namespace ToyShop.Web.Pages;

public class IndexModel : PageModel
{
    private readonly ProductCatalog _catalog;

    public IndexModel(ProductCatalog catalog)
    {
        _catalog = catalog;
    }

    public IReadOnlyList<string> Categories { get; private set; } = Array.Empty<string>();

    public IReadOnlyList<Product> Products { get; private set; } = Array.Empty<Product>();

    public string? SelectedCategory { get; private set; }

    public void OnGet(string? category)
    {
        Categories = _catalog.Categories();

        if (!string.IsNullOrWhiteSpace(category) && Categories.Contains(category))
        {
            SelectedCategory = category;
            Products = _catalog.ByCategory(category);
        }
        else
        {
            Products = _catalog.All();
        }
    }
}
