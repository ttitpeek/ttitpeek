using System.Linq;
using System.Text.RegularExpressions;

namespace TaskModule3.Services;

public class Validation
{
    public static bool IsValidPhone(string phone)
    {
        return !string.IsNullOrWhiteSpace(phone) && Regex.IsMatch(phone, @"^\+7\d{10}$");
    }

    public static bool IsValidLogin(string login)
    {
        return !string.IsNullOrWhiteSpace(login) && !Service.GetDbContext().Users.Any(u => u.Login == login);
    }
}