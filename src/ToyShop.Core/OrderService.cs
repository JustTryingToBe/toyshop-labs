using System.Collections.Concurrent;

namespace ToyShop.Core;

public sealed record Order(int Id, string CustomerName, decimal Total, int ItemCount, DateTime CreatedAt);

/// <summary>Хранилище заказов (в памяти, потокобезопасное).</summary>
public class OrderService
{
    private readonly ConcurrentDictionary<int, Order> _orders = new();
    private int _lastId;

    public int Count => _orders.Count;

    public Order Place(Cart cart, CustomerInfo customer)
    {
        ArgumentNullException.ThrowIfNull(cart);
        ArgumentNullException.ThrowIfNull(customer);

        if (cart.IsEmpty)
        {
            throw new InvalidOperationException("Нельзя оформить пустую корзину.");
        }

        var errors = CheckoutValidator.Validate(customer);
        if (errors.Count > 0)
        {
            throw new ArgumentException(string.Join(" ", errors), nameof(customer));
        }

        var id = Interlocked.Increment(ref _lastId);
        var order = new Order(id, customer.Name!.Trim(), cart.Total, cart.ItemCount, DateTime.UtcNow);
        _orders[id] = order;
        return order;
    }

    public Order? Find(int id) => _orders.TryGetValue(id, out var order) ? order : null;
}
