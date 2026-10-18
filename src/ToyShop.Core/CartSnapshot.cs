namespace ToyShop.Core;

/// <summary>Состояние корзины для сохранения в сессии.</summary>
public sealed record CartItemSnapshot(int ProductId, int Quantity);

public sealed record CartSnapshot(List<CartItemSnapshot> Items, string? PromoCode);
