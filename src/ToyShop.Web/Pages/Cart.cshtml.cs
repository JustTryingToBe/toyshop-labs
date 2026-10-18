using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ToyShop.Core;

namespace ToyShop.Web.Pages;

public class CartModel : PageModel
{
    private readonly CartStore _cartStore;

    public CartModel(CartStore cartStore)
    {
        _cartStore = cartStore;
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

    private Cart LoadCart() => _cartStore.Load(HttpContext.Session);

    private void SaveCart(Cart cart) => _cartStore.Save(HttpContext.Session, cart);
}