namespace SafeVault.Services;

public class InputValidator
{
    public static bool ValidateInput(string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return false;
        input = System.Web.HttpUtility.HtmlEncode(input);
        return input.Length <= 100 && !input.Contains("--") && !input.Contains(";");
    }

    public static bool ValidateEmail(string email)
    {
        return !string.IsNullOrWhiteSpace(email) &&
               System.Net.Mail.MailAddress.TryCreate(email, out _);
    }
}
