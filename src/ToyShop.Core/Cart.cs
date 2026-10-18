namespace ToyShop.Core;

public sealed class CartLine
{
    public CartLine(Product product, int quantity)
    {
        Product = product;
        Quantity = quantity;
    }

    public Product Product { get; }

    public int Quantity { get; internal set; }

    public decimal LineTotal => Product.Price * Quantity;
}

/// <summary>Корзина покупателя.</summary>
public class Cart
{
    public const decimal FreeShippingThreshold = 3000m;
    public const decimal ShippingCost = 300m;
    public const string PromoCode = "STUDENT10";
    public const decimal PromoRate = 0.10m;

    private readonly List<CartLine> _lines = new();

    public IReadOnlyList<CartLine> Lines => _lines;

    public string? AppliedPromoCode { get; private set; }

    public bool IsEmpty => _lines.Count == 0;

    public int ItemCount => _lines.Sum(l => l.Quantity);

    public void Add(Product product, int quantity = 1)
    {
        ArgumentNullException.ThrowIfNull(product);
        if (quantity <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(quantity), "Количество должно быть больше нуля.");
        }

        var line = _lines.FirstOrDefault(l => l.Product.Id == product.Id);
        if (line is null)
        {
            _lines.Add(new CartLine(product, quantity));
        }
        else
        {
            line.Quantity += quantity;
        }
    }

    public void ChangeQuantity(int productId, int quantity)
    {
        var line = _lines.FirstOrDefault(l => l.Product.Id == productId)
                   ?? throw new InvalidOperationException($"Товара {productId} нет в корзине.");

        if (quantity <= 0)
        {
            _lines.Remove(line);
        }
        else
        {
            line.Quantity = quantity;
        }
    }

    public bool Remove(int productId) => _lines.RemoveAll(l => l.Product.Id == productId) > 0;

    public void Clear()
    {
        _lines.Clear();
        AppliedPromoCode = null;
    }

    public bool ApplyPromo(string? code)
    {
        if (string.Equals(code?.Trim(), PromoCode, StringComparison.OrdinalIgnoreCase))
        {
            AppliedPromoCode = PromoCode;
            return true;
        }

        return false;
    }

    public decimal Subtotal => _lines.Sum(l => l.LineTotal);

    public decimal Discount => AppliedPromoCode is null ? 0m : Math.Round(Subtotal * PromoRate, 2);

    // Требование заказчика: если товаров на 3000 ₽ и больше, доставка бесплатная.
    public decimal Shipping
    {
        get
        {
            if (IsEmpty)
            {
                return 0m;
            }

            return Subtotal >= FreeShippingThreshold ? 0m : ShippingCost;
        }
    }

    public decimal Total => Subtotal - Discount + Shipping;

    public CartSnapshot ToSnapshot() => new(
        _lines.Select(l => new CartItemSnapshot(l.Product.Id, l.Quantity)).ToList(),
        AppliedPromoCode);

    public static Cart FromSnapshot(CartSnapshot snapshot, ProductCatalog catalog)
    {
        ArgumentNullException.ThrowIfNull(snapshot);
        ArgumentNullException.ThrowIfNull(catalog);

        var cart = new Cart();
        foreach (var item in snapshot.Items)
        {
            var product = catalog.Find(item.ProductId);
            if (product is not null && item.Quantity > 0)
            {
                cart.Add(product, item.Quantity);
            }
        }

        cart.ApplyPromo(snapshot.PromoCode);
        return cart;
    }
}
