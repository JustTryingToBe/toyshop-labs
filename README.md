# Магазин игрушек — учебный проект для лабораторных по ALM

Учебное веб-приложение на ASP.NET Core (Razor Pages, .NET 10), на котором выполняются лабораторные:
Git, юнит-тестирование и покрытие кода, UI-тесты (Playwright), нагрузочное тестирование (k6), CI/CD (GitHub Actions).

## Запуск

```
dotnet run --project src/ToyShop.Web
```

Сайт откроется на http://localhost:5080

## Тесты

```
dotnet test tests/ToyShop.Core.Tests
```

Промокод для проверки скидки: `STUDENT10`.
