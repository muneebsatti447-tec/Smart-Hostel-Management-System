namespace SmartHostel.Utilities
{
    public static class ConsoleHelper
    {
        private const string LINE = "════════════════════════════════════════════════════════════════";
        private const string THIN = "────────────────────────────────────────────────────────────────";

        public static void Banner()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(LINE);
            Console.WriteLine("       🏢  SMART HOSTEL MANAGEMENT SYSTEM  🏢");
            Console.WriteLine("           University Hostel Administration");
            Console.WriteLine(LINE);
            Console.ResetColor();
        }

        public static void Header(string title)
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(THIN);
            Console.WriteLine($"  {title.ToUpper()}");
            Console.WriteLine(THIN);
            Console.ResetColor();
        }

        public static void Success(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\n  ✓ {msg}");
            Console.ResetColor();
        }

        public static void Error(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"\n  ✗ {msg}");
            Console.ResetColor();
        }

        public static void Warning(string msg)
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine($"\n  ⚠ {msg}");
            Console.ResetColor();
        }

        public static void Info(string msg)
        {
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine($"  ℹ {msg}");
            Console.ResetColor();
        }

        public static void PrintLine(string txt = "") => Console.WriteLine("  " + txt);

        public static string ReadInput(string prompt)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write($"  ► {prompt}: ");
            Console.ResetColor();
            return Console.ReadLine()?.Trim() ?? "";
        }

        public static int ReadMenuChoice(int max)
        {
            while (true)
            {
                Console.Write("\n  ► Enter choice [1-{0}] or [0] to go back: ", max);
                if (int.TryParse(Console.ReadLine(), out int c) && c >= 0 && c <= max)
                    return c;
                Error("Invalid choice. Try again.");
            }
        }

        public static void Pause()
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write("\n  Press any key to continue...");
            Console.ResetColor();
            Console.ReadKey(true);
        }

        public static bool Confirm(string prompt)
        {
            Console.Write($"\n  ► {prompt} (y/n): ");
            return Console.ReadLine()?.Trim().ToLower() == "y";
        }

        public static void Table(string[] headers, List<string[]> rows)
        {
            // Calculate column widths
            int[] widths = headers.Select((h, i) =>
                Math.Max(h.Length, rows.Count > 0 ? rows.Max(r => i < r.Length ? r[i].Length : 0) : 0) + 2
            ).ToArray();

            string Sep() => "  +" + string.Join("+", widths.Select(w => new string('-', w))) + "+";
            string Row(string[] cells) => "  |" + string.Join("|", cells.Select((c, i) => " " + c.PadRight(widths[i] - 1))) + "|";

            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine(Sep());
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(Row(headers));
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine(Sep());
            Console.ResetColor();

            foreach (var row in rows)
            {
                Console.WriteLine(Row(row));
            }
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine(Sep());
            Console.ResetColor();
        }
    }

    public static class Validator
    {
        public static bool IsValidCNIC(string cnic) =>
            System.Text.RegularExpressions.Regex.IsMatch(cnic, @"^\d{5}-\d{7}-\d{1}$");

        public static bool IsValidPhone(string phone) =>
            System.Text.RegularExpressions.Regex.IsMatch(phone, @"^0\d{3}-\d{7}$");

        public static bool IsValidEmail(string email) =>
            System.Text.RegularExpressions.Regex.IsMatch(email, @"^[^@]+@[^@]+\.[^@]+$");

        public static bool IsNotEmpty(string value) => !string.IsNullOrWhiteSpace(value);

        public static bool IsValidGender(string gender) =>
            gender.Equals("Male", StringComparison.OrdinalIgnoreCase) ||
            gender.Equals("Female", StringComparison.OrdinalIgnoreCase);
    }
}
