using SmartHostel.Data;
using SmartHostel.Utilities;

namespace SmartHostel.Services
{
    public enum UserRole { None, Admin, Warden }

    public class AuthService
    {
        public UserRole CurrentRole { get; private set; } = UserRole.None;

        public bool Login()
        {
            ConsoleHelper.Header("Login");
            ConsoleHelper.PrintLine("1. Admin Login");
            ConsoleHelper.PrintLine("2. Warden Login");
            int choice = ConsoleHelper.ReadMenuChoice(2);
            if (choice == 0) return false;

            string username = ConsoleHelper.ReadInput("Username");
            string password = ReadPassword("Password");

            if (choice == 1)
            {
                if (username == DataStore.AdminUser.Id && DataStore.AdminUser.Authenticate(password))
                {
                    CurrentRole = UserRole.Admin;
                    ConsoleHelper.Success($"Welcome, {DataStore.AdminUser.Name}!");
                    return true;
                }
            }
            else
            {
                if (username == DataStore.WardenUser.Id && DataStore.WardenUser.Authenticate(password))
                {
                    CurrentRole = UserRole.Warden;
                    ConsoleHelper.Success($"Welcome, {DataStore.WardenUser.Name}!");
                    return true;
                }
            }

            ConsoleHelper.Error("Invalid credentials. Access denied.");
            return false;
        }

        public void Logout()
        {
            ConsoleHelper.Info($"Logged out from {CurrentRole} session.");
            CurrentRole = UserRole.None;
        }

        private static string ReadPassword(string prompt)
        {
            Console.Write($"  ► {prompt}: ");
            string pwd = "";
            ConsoleKeyInfo key;
            do
            {
                key = Console.ReadKey(true);
                if (key.Key != ConsoleKey.Backspace && key.Key != ConsoleKey.Enter)
                { pwd += key.KeyChar; Console.Write("*"); }
                else if (key.Key == ConsoleKey.Backspace && pwd.Length > 0)
                { pwd = pwd[..^1]; Console.Write("\b \b"); }
            } while (key.Key != ConsoleKey.Enter);
            Console.WriteLine();
            return pwd;
        }
    }
}
