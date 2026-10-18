using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ToyShop.Core;

namespace ToyShop.Web.Pages;

public class CheckoutModel : PageModel
{
    private readonly ProductCatalog _catalog;
    private readonly OrderService _orders;

    public CheckoutModel(ProductCatalog catalog, OrderService orders)
    {
        _catalog = catalog;
        _orders = orders;
    }

    [BindProperty]
    public CustomerInfo Customer { get; set; } = new();

    public Cart Cart { get; private set; } = new();

    public IReadOnlyList<string> Errors { get; private set; } = Array.Empty<string>();

    public void OnGet()
    {
        Cart = LoadCart();
    }

    public IActionResult OnPost()
    {
        Cart = LoadCart();
        if (Cart.IsEmpty)
        {
            return RedirectToPage("/Cart");
        }

        Errors = CheckoutValidator.Validate(Customer);
        if (Errors.Count > 0)
        {
            return Page();
        }

        var order = _orders.Place(Cart, Customer);
        Cart.Clear();
        SaveCart(Cart);
        return RedirectToPage("/Order", new { id = order.Id });
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
