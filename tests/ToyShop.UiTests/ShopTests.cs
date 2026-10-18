using NUnit.Framework;
using System.Text.RegularExpressions;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;

namespace ToyShop.UiTests;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class ShopTests : PageTest
{
    private const string BaseUrl = "http://localhost:5080";

    // Упражнение 1: код получен записью действий (codegen) и немного причёсан.
    [Test]
    public async Task AddGliderToCart_ShowsLineAndTotal()
    {
        await Page.GotoAsync(BaseUrl);
        await Page.GetByRole(AriaRole.Link, new() { Name = "Самолёты", Exact = true }).ClickAsync();
        await Page.GetByText("Планер «Альбатрос»").ClickAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "В корзину" }).ClickAsync();

        await Expect(Page).ToHaveURLAsync(new Regex("/Cart$"));
        await Expect(Page.GetByTestId("cart-line")).ToHaveCountAsync(1);
        await Expect(Page.GetByTestId("total")).ToHaveTextAsync("1590 ₽");
    }

    // Упражнение 2: проверки (assertions), добавленные через инспектор.
    [Test]
    public async Task PromoCode_GivesTenPercentDiscount()
    {
        await Page.GotoAsync($"{BaseUrl}/Product/2");
        await Page.GetByRole(AriaRole.Button, new() { Name = "В корзину" }).ClickAsync();
        await Page.GetByLabel("Промокод").FillAsync("STUDENT10");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Применить" }).ClickAsync();

        await Expect(Page.GetByTestId("promo-message")).ToContainTextAsync("Промокод применён");
        await Expect(Page.GetByTestId("discount")).ToHaveTextAsync("129 ₽");
        await Expect(Page.GetByTestId("total")).ToHaveTextAsync("1461 ₽");
    }

    [Test]
    public async Task Checkout_WithInvalidEmail_ShowsError()
    {
        await Page.GotoAsync($"{BaseUrl}/Product/5");
        await Page.GetByRole(AriaRole.Button, new() { Name = "В корзину" }).ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Оформить заказ" }).ClickAsync();

        await Page.GetByLabel("Имя").FillAsync("Иван");
        await Page.GetByLabel("E-mail").FillAsync("ivan-mail.ru");
        await Page.GetByLabel("Телефон").FillAsync("89123456789");
        await Page.GetByLabel("Адрес доставки").FillAsync("г. Казань, ул. Пушкина, 1");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Подтвердить заказ" }).ClickAsync();

        await Expect(Page.GetByTestId("errors")).ToContainTextAsync("e-mail");
        await Page.ScreenshotAsync(new() { Path = "checkout-error.png", FullPage = true });
    }

    [Test]
    public async Task Checkout_WithValidData_CreatesOrder()
    {
        await Page.GotoAsync($"{BaseUrl}/Product/7");
        await Page.GetByRole(AriaRole.Button, new() { Name = "В корзину" }).ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Оформить заказ" }).ClickAsync();

        await Page.GetByLabel("Имя").FillAsync("Иван");
        await Page.GetByLabel("E-mail").FillAsync("ivan@example.com");
        await Page.GetByLabel("Телефон").FillAsync("+7 (912) 345-67-89");
        await Page.GetByLabel("Адрес доставки").FillAsync("г. Казань, ул. Пушкина, 1");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Подтвердить заказ" }).ClickAsync();

        await Expect(Page.GetByTestId("order-confirmation")).ToContainTextAsync("оформлен");
        await Expect(Page.GetByTestId("order-total")).ToHaveTextAsync("2790 ₽");
    }

    // Упражнение 3: тест на основе данных из CSV-файла.
    public static IEnumerable<TestCaseData> CartCases()
    {
        var path = Path.Combine(AppContext.BaseDirectory, "products.csv");
        foreach (var line in File.ReadAllLines(path).Skip(1))
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            var parts = line.Split(',');
            yield return new TestCaseData(int.Parse(parts[0]), int.Parse(parts[1]), parts[2].Trim())
                .SetName($"CartTotal_Product{parts[0]}_x{parts[1]}");
        }
    }

    [TestCaseSource(nameof(CartCases))]
    public async Task CartTotal_IsCalculatedCorrectly(int productId, int quantity, string expectedTotal)
    {
        await Page.GotoAsync($"{BaseUrl}/Product/{productId}");
        await Page.GetByLabel("Количество").FillAsync(quantity.ToString());
        await Page.GetByRole(AriaRole.Button, new() { Name = "В корзину" }).ClickAsync();

        await Expect(Page.GetByTestId("total")).ToHaveTextAsync(expectedTotal);
    }
}
