using System.Text.Encodings.Web;
using System.Text.Unicode;
using Microsoft.Extensions.WebEncoders;
using ToyShop.Core;
using ToyShop.Web;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();

// Кириллица в HTML без замены на &#x...; — так исходный код страниц читаемый.
builder.Services.Configure<WebEncoderOptions>(options =>
    options.TextEncoderSettings = new TextEncoderSettings(UnicodeRanges.All));

// Корзина хранится в сессии пользователя.
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddSingleton<ProductCatalog>();
builder.Services.AddSingleton<OrderService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

app.UseRouting();
app.UseSession();

app.MapRazorPages();

// Служебная точка для проверки после развёртывания (используется в CI/CD).
app.MapGet("/health", () => Results.Ok(new { status = "ok", version = AppInfo.Version }));

app.Run();
