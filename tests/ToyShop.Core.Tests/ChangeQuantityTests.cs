namespace ToyShop.Core.Tests;

public class ChangeQuantityTests
{
    private static readonly Product Plane = new(1, "Планер", "Самолёты", 1290m, "", "✈️");

    [Fact]
    [Trait("Category", "Cart")]
    public void ChangeQuantity_ToZero_RemovesLine()
    {
        var cart = new Cart();
        cart.Add(Plane, 3);

        cart.ChangeQuantity(Plane.Id, 0);

        Assert.True(cart.IsEmpty);
    }

    [Fact]
    [Trait("Category", "Cart")]
    public void ChangeQuantity_UnknownProduct_Throws()
    {
        var cart = new Cart();

        Assert.Throws<InvalidOperationException>(() => cart.ChangeQuantity(42, 1));
    }
}