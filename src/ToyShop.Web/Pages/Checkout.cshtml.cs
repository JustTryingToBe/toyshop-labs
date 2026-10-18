using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ToyShop.Core;

namespace ToyShop.Web.Pages;

public class CheckoutModel : PageModel
{
    private readonly CartStore _cartStore;
    private readonly OrderService _orders;

    public CheckoutModel(CartStore cartStore, OrderService orders)
    {
        _cartStore = cartStore;
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

    private Cart LoadCart() => _cartStore.Load(HttpContext.Session);

    private void SaveCart(Cart cart) => _cartStore.Save(HttpContext.Session, cart);
}