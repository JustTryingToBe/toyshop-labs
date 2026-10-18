using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ToyShop.Core;

namespace ToyShop.Web.Pages;

public class CartModel : PageModel
{
    private readonly ProductCatalog _catalog;

    public CartModel(ProductCatalog catalog)
    {
        _catalog = catalog;
    }

    public Cart Cart { get; private set; } = new();

    [TempData]
    public string? Message { get; set; }

    public void OnGet()
    {
        Cart = LoadCart();
    }

    public IActionResult OnPostRemove(int id)
    {
        var cart = LoadCart();
        cart.Remove(id);
        SaveCart(cart);
        return RedirectToPage();
    }

    public IActionResult OnPostPromo(string? promoCode)
    {
        var cart = LoadCart();
        Message = cart.ApplyPromo(promoCode)
            ? "Промокод применён: скидка 10%."
            : "Такого промокода нет.";
        SaveCart(cart);
        return RedirectToPage();
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
