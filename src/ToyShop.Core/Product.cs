namespace ToyShop.Core;

/// <summary>Товар в каталоге магазина.</summary>
public sealed record Product(int Id, string Name, string Category, decimal Price, string Description, string Emoji);
