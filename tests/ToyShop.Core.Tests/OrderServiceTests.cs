namespace ToyShop.Core.Tests;

public class OrderServiceTests
{
    private static CustomerInfo Customer() => new()
    {
        Name = "Иван",
        Email = "ivan@example.com",
        Phone = "89123456789",
        Address = "г. Казань, ул. Пушкина, 1",
    };

    [Fact]
    [Trait("Category", "Orders")]
    public void Place_ValidOrder_AssignsIncreasingIds()
    {
        var cart = new Cart();
        cart.Add(new ProductCatalog().Find(1)!);
        var service = new OrderService();

        var first = service.Place(cart, Customer());
        var second = service.Place(cart, Customer());

        Assert.Equal(1, first.Id);
        Assert.Equal(2, second.Id);
        Assert.Equal(2, service.Count);
        Assert.Same(first, service.Find(1));
    }

    [Fact]
    [Trait("Category", "Orders")]
    public void Place_EmptyCart_Throws()
    {
        var service = new OrderService();

        Assert.Throws<InvalidOperationException>(() => service.Place(new Cart(), Customer()));
    }

    [Fact]
    [Trait("Category", "Orders")]
    public void Place_InvalidCustomer_Throws()
    {
        var cart = new Cart();
        cart.Add(new ProductCatalog().Find(1)!);

        Assert.Throws<ArgumentException>(() => new OrderService().Place(cart, new CustomerInfo()));
    }
}