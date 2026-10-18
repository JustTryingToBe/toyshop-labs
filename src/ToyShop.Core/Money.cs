using System.Globalization;

namespace ToyShop.Core;

public static class Money
{
    /// <summary>Формат цены без разделителей разрядов: 1290 ₽.</summary>
    public static string Format(decimal amount) =>
        amount.ToString("0.##", CultureInfo.InvariantCulture) + " ₽";
}
