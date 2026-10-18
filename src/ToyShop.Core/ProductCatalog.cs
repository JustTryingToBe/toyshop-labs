namespace ToyShop.Core;

/// <summary>Каталог товаров (хранится в памяти).</summary>
public class ProductCatalog
{
    private readonly List<Product> _products = new()
    {
        new(1, "Бумажный самолёт «Стриж»", "Самолёты", 290m, "Набор для сборки бумажного самолёта с выкройками.", "✈️"),
        new(2, "Планер «Альбатрос»", "Самолёты", 1290m, "Метательный планер из пенопласта, размах крыльев 48 см.", "🛩️"),
        new(3, "Радиоуправляемый самолёт «Сокол»", "Самолёты", 4590m, "Самолёт на пульте управления, до 15 минут полёта.", "🛫"),
        new(4, "Гоночная машинка «Молния»", "Машинки", 890m, "Инерционная машинка, масштаб 1:32.", "🏎️"),
        new(5, "Пожарная машина", "Машинки", 1500m, "Пожарная машина с выдвижной лестницей и сиреной.", "🚒"),
        new(6, "Внедорожник на пульте", "Машинки", 3290m, "Полный привод, аккумулятор, едет даже по траве.", "🚙"),
        new(7, "Конструктор «Космодром»", "Конструкторы", 2490m, "312 деталей: ракета, стартовый стол и марсоход.", "🚀"),
        new(8, "Конструктор «Робот»", "Конструкторы", 1990m, "Собери робота, который ходит и мигает глазами.", "🤖"),
        new(9, "Набор «Маленький инженер»", "Конструкторы", 750m, "Деревянные детали, винты и отвёртка.", "🔧"),
    };

    public IReadOnlyList<Product> All() => _products;

    public IReadOnlyList<string> Categories() =>
        _products.Select(p => p.Category).Distinct().ToList();

    public IReadOnlyList<Product> ByCategory(string category) =>
        _products.Where(p => string.Equals(p.Category, category, StringComparison.OrdinalIgnoreCase)).ToList();

    public Product? Find(int id) => _products.FirstOrDefault(p => p.Id == id);

    /// <summary>Товары из той же категории — блок «Вам также может понравиться».</summary>
    public IReadOnlyList<Product> Related(int productId, int count = 2)
    {
        var product = Find(productId);
        if (product is null)
        {
            return Array.Empty<Product>();
        }

        return _products
            .Where(p => p.Category == product.Category && p.Id != productId)
            .Take(count)
            .ToList();
    }
}
