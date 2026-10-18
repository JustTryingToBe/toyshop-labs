using System.Text.Json;
using ToyShop.Core;

namespace ToyShop.Web;

/// <summary>Загрузка и сохранение корзины в сессии. Вынесено из трёх страниц после поиска дублирующегося кода.</summary>
public class CartStore
{
    private const string SessionKey = "cart";
    private readonly ProductCatalog _catalog;

    public CartStore(ProductCatalog catalog)
    {
        _catalog = catalog;
    }

    public Cart Load(ISession session)
    {
        var json = session.GetString(SessionKey);
        if (string.IsNullOrEmpty(json))
        {
            return new Cart();
        }

        var snapshot = JsonSerializer.Deserialize<CartSnapshot>(json);
        return snapshot is null ? new Cart() : Cart.FromSnapshot(snapshot, _catalog);
    }

    public void Save(ISession session, Cart cart)
    {
        session.SetString(SessionKey, JsonSerializer.Serialize(cart.ToSnapshot()));
    }
}