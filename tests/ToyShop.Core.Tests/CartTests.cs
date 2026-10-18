namespace ToyShop.Core.Tests;

public class CartTests
{
    private static readonly Product Plane = new(1, "Планер", "Самолёты", 1290m, "", "✈️");
    private static readonly Product Truck = new(2, "Пожарная машина", "Машинки", 1500m, "", "🚒");
    private static readonly Product Kit = new(3, "Набор инженера", "Конструкторы", 750m, "", "🔧");

    [Fact]
    [Trait("Category", "Cart")]
    public void Add_NewProduct_CreatesLine()
    {
        var cart = new Cart();

        cart.Add(Plane);

        var line = Assert.Single(cart.Lines);
        Assert.Equal(Plane.Id, line.Product.Id);
        Assert.Equal(1, line.Quantity);
    }

    [Fact]
    [Trait("Category", "Cart")]
    public void Add_SameProductTwice_IncreasesQuantity()
    {
        var cart = new Cart();

        cart.Add(Plane);
        cart.Add(Plane, 2);

        var line = Assert.Single(cart.Lines);
        Assert.Equal(3, line.Quantity);
    }

    [Theory]
    [Trait("Category", "Cart")]
    [InlineData(0)]
    [InlineData(-1)]
    public void Add_NotPositiveQuantity_Throws(int quantity)
    {
        var cart = new Cart();

        Assert.Throws<ArgumentOutOfRangeException>(() => cart.Add(Plane, quantity));
    }

    [Fact]
    [Trait("Category", "Cart")]
    public void Remove_ExistingProduct_RemovesLine()
    {
        var cart = new Cart();
        cart.Add(Plane);
        cart.Add(Kit);

        var removed = cart.Remove(Plane.Id);

        Assert.True(removed);
        var line = Assert.Single(cart.Lines);
        Assert.Equal(Kit.Id, line.Product.Id);
    }

    [Fact]
    [Trait("Category", "Cart")]
    public void Subtotal_SumsAllLines()
    {
        var cart = new Cart();
        cart.Add(Plane);
        cart.Add(Kit, 2);

        Assert.Equal(1290m + 750m * 2, cart.Subtotal);
    }

    [Fact]
    [Trait("Category", "Shipping")]
    public void Shipping_EmptyCart_IsZero()
    {
        var cart = new Cart();

        Assert.Equal(0m, cart.Shipping);
    }

    [Fact]
    [Trait("Category", "Shipping")]
    public void Shipping_BelowThreshold_IsCharged()
    {
        var cart = new Cart();
        cart.Add(Plane);

        Assert.Equal(Cart.ShippingCost, cart.Shipping);
    }

    [Fact]
    [Trait("Category", "Shipping")]
    public void Shipping_ExactlyThreshold_IsFree()
    {
        var cart = new Cart();
        cart.Add(Truck, 2); // ровно 3000 ₽

        Assert.Equal(0m, cart.Shipping);
    }

    [Fact]
    [Trait("Category", "Shipping")]
    public void Shipping_AboveThreshold_IsFree()
    {
        var cart = new Cart();
        cart.Add(Truck, 2);
        cart.Add(Kit);

        Assert.Equal(0m, cart.Shipping);
    }

    [Theory]
    [Trait("Category", "Promo")]
    [InlineData("STUDENT10")]
    [InlineData("student10")]
    [InlineData("  STUDENT10  ")]
    public void ApplyPromo_ValidCode_Gives10PercentDiscount(string code)
    {
        var cart = new Cart();
        cart.Add(Plane);

        var applied = cart.ApplyPromo(code);

        Assert.True(applied);
        Assert.Equal(130m, cart.Discount);
    }

    [Fact]
    [Trait("Category", "Promo")]
    public void ApplyPromo_UnknownCode_ReturnsFalse()
    {
        var cart = new Cart();
        cart.Add(Plane);

        var applied = cart.ApplyPromo("FREE100");

        Assert.False(applied);
        Assert.Equal(0m, cart.Discount);
    }

    [Fact]
    [Trait("Category", "Cart")]
    public void Total_IsSubtotalMinusDiscountPlusShipping()
    {
        var cart = new Cart();
        cart.Add(Plane);
        cart.ApplyPromo("STUDENT10");

        Assert.Equal(1290m - 129m + 300m, cart.Total);
    }

    [Fact]
    [Trait("Category", "Cart")]
    public void Snapshot_RoundTrip_RestoresCart()
    {
        var catalog = new ProductCatalog();
        var cart = new Cart();
        cart.Add(catalog.Find(2)!, 2);
        cart.Add(catalog.Find(7)!);
        cart.ApplyPromo("STUDENT10");

        var restored = Cart.FromSnapshot(cart.ToSnapshot(), catalog);

        Assert.Equal(cart.ItemCount, restored.ItemCount);
        Assert.Equal(cart.Total, restored.Total);
        Assert.Equal(Cart.PromoCode, restored.AppliedPromoCode);
    }
}
