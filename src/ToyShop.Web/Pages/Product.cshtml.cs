using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ToyShop.Core;

namespace ToyShop.Web.Pages;

public class ProductModel : PageModel
{
    private readonly ProductCatalog _catalog;

    public ProductModel(ProductCatalog catalog)
    {
        _catalog = catalog;
    }

    public Product Product { get; private set; } = null!;

    public IReadOnlyList<Product> Related { get; private set; } = Array.Empty<Product>();

    public IActionResult OnGet(int id)
    {
        var product = _catalog.Find(id);
        if (product is null)
        {
            return NotFound();
        }

        Product = product;
        Related = _catalog.Related(id);
        return Page();
    }

    public IActionResult OnPostAdd(int id, int quantity = 1)
    {
        var product = _catalog.Find(id);
        if (product is null)
        {
            return NotFound();
        }

        var cart = LoadCart();
        cart.Add(product, Math.Clamp(quantity, 1, 99));
        SaveCart(cart);
        return RedirectToPage("/Cart");
    }

    private Cart LoadCart()
    {
        var json = HttpContext.Session.GetString("cart");
        if (string.IsNullOrEmpty(json))
        {
            return new Cart();
        }

        var snapshot = JsonSerializer.Deserialize<CartSnapshot>(json);
        if (snapshot is null)
        {
            return new Cart();
        }

        return Cart.FromSnapshot(snapshot, _catalog);
    }

    private void SaveCart(Cart cart)
    {
        var json = JsonSerializer.Serialize(cart.ToSnapshot());
        HttpContext.Session.SetString("cart", json);
    }
}
