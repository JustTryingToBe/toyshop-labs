namespace ToyShop.Core.Tests;

public class CheckoutValidatorTests
{
    private static CustomerInfo ValidCustomer() => new()
    {
        Name = "Иван",
        Email = "ivan@example.com",
        Phone = "89123456789",
        Address = "г. Казань, ул. Пушкина, 1",
    };

    [Fact]
    [Trait("Category", "Validation")]
    public void Validate_CorrectCustomer_NoErrors()
    {
        var errors = CheckoutValidator.Validate(ValidCustomer());

        Assert.Empty(errors);
    }

    [Theory]
    [Trait("Category", "Validation")]
    [InlineData("")]
    [InlineData("ivan")]
    [InlineData("ivan@")]
    [InlineData("ivan@mail")]
    [InlineData("ivan petrov@mail.ru")]
    public void Validate_BadEmail_ReturnsEmailError(string email)
    {
        var customer = ValidCustomer();
        customer.Email = email;

        var errors = CheckoutValidator.Validate(customer);

        Assert.Contains(errors, e => e.Contains("e-mail"));
    }

    [Fact]
    [Trait("Category", "Validation")]
    public void Validate_PhoneWithFormatting_IsAccepted()
    {
        var customer = ValidCustomer();
        customer.Phone = "+7 (912) 345-67-89";

        var errors = CheckoutValidator.Validate(customer);

        Assert.Empty(errors);
    }

    [Fact]
    [Trait("Category", "Validation")]
    public void Validate_EmptyCustomer_ReturnsFourErrors()
    {
        var errors = CheckoutValidator.Validate(new CustomerInfo());

        Assert.Equal(4, errors.Count);
    }
}
