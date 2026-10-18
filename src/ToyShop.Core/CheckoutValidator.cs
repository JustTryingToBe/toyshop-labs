using System.Text.RegularExpressions;

namespace ToyShop.Core;

public static class CheckoutValidator
{
    private static readonly Regex EmailRegex = new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);

    public static IReadOnlyList<string> Validate(CustomerInfo customer)
    {
        ArgumentNullException.ThrowIfNull(customer);
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(customer.Name) || customer.Name.Trim().Length < 2)
        {
            errors.Add("Укажите имя (не короче 2 символов).");
        }

        if (string.IsNullOrWhiteSpace(customer.Email) || !EmailRegex.IsMatch(customer.Email.Trim()))
        {
            errors.Add("Укажите корректный e-mail.");
        }

        var digits = new string((customer.Phone ?? string.Empty).Where(char.IsDigit).ToArray());
        if (digits.Length is < 10 or > 11)
        {
            errors.Add("Телефон должен содержать 10–11 цифр.");
        }

        if (string.IsNullOrWhiteSpace(customer.Address))
        {
            errors.Add("Укажите адрес доставки.");
        }

        return errors;
    }
}
