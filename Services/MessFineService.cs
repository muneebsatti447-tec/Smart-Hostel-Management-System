using SmartHostel.Data;
using SmartHostel.Models;
using SmartHostel.Utilities;

namespace SmartHostel.Services
{
    public class MessService
    {
        public void GenerateBill()
        {
            ConsoleHelper.Header("Generate Mess Bill");
            string studentId = ConsoleHelper.ReadInput("Student ID");
            var student = DataStore.Students.FirstOrDefault(s => s.Id.Equals(studentId, StringComparison.OrdinalIgnoreCase));
            if (student == null) { ConsoleHelper.Error("Student not found."); return; }

            int month = DateTime.Now.Month;
            int year  = DateTime.Now.Year;

            if (DataStore.MessBills.Any(b => b.StudentId == studentId && b.Month == month && b.Year == year))
            { ConsoleHelper.Warning("Bill already generated for this month."); return; }

            string mealsStr = ConsoleHelper.ReadInput("Total meals consumed this month");
            if (!int.TryParse(mealsStr, out int meals) || meals < 0)
            { ConsoleHelper.Error("Invalid meal count."); return; }

            decimal penalty = student.MessDues > 3000m ? student.MessDues * 0.05m : 0m;

            var bill = new MessBill(DataStore.NextBillId(), student.Id, student.Name, month, year)
            {
                MealsConsumed = meals,
                Penalty = penalty
            };

            DataStore.MessBills.Add(bill);
            student.MessDues = bill.TotalAmount;
            DataStore.SaveAll();

            Console.WriteLine(bill.GenerateReport());

            // Notify student
            var ns = new NotificationService();
            ns.Send(student.Id, "Mess Bill Generated",
                $"Your mess bill for {new DateTime(year, month, 1):MMMM yyyy} is Rs.{bill.TotalAmount:N0}. Please pay by the 10th.");
        }

        public void ViewAllBills()
        {
            ConsoleHelper.Header("All Mess Bills");
            if (!DataStore.MessBills.Any()) { ConsoleHelper.Warning("No bills found."); return; }

            var headers = new[] { "Bill ID", "Student", "Month", "Meals", "Penalty", "Total", "Paid?" };
            var rows = DataStore.MessBills.Select(b => new[]
            {
                b.BillId, b.StudentName, $"{new DateTime(b.Year,b.Month,1):MMM yyyy}",
                b.MealsConsumed.ToString(), b.Penalty.ToString("N0"),
                b.TotalAmount.ToString("N0"), b.IsPaid ? "Yes" : "No"
            }).ToList();

            ConsoleHelper.Table(headers, rows);
        }

        public void MarkAsPaid()
        {
            ConsoleHelper.Header("Mark Mess Bill as Paid");
            string billId = ConsoleHelper.ReadInput("Bill ID");
            var bill = DataStore.MessBills.FirstOrDefault(b => b.BillId.Equals(billId, StringComparison.OrdinalIgnoreCase));
            if (bill == null) { ConsoleHelper.Error("Bill not found."); return; }

            if (bill.IsPaid) { ConsoleHelper.Warning("This bill is already paid."); return; }

            bill.IsPaid = true;

            var student = DataStore.Students.FirstOrDefault(s => s.Id == bill.StudentId);
            if (student != null) student.MessDues = 0;

            DataStore.SaveAll();
            ConsoleHelper.Success($"Bill {billId} marked as paid. Mess dues cleared for {bill.StudentName}.");
        }

        public void MonthlyRevenue()
        {
            ConsoleHelper.Header("Monthly Mess Revenue");
            var groupedByMonth = DataStore.MessBills
                .GroupBy(b => new { b.Year, b.Month })
                .OrderByDescending(g => g.Key.Year).ThenByDescending(g => g.Key.Month);

            foreach (var g in groupedByMonth)
            {
                decimal total   = g.Sum(b => b.TotalAmount);
                decimal collected = g.Where(b => b.IsPaid).Sum(b => b.TotalAmount);
                decimal pending = total - collected;
                ConsoleHelper.PrintLine($"  {new DateTime(g.Key.Year, g.Key.Month, 1):MMMM yyyy}  |  Total: Rs.{total:N0}  |  Collected: Rs.{collected:N0}  |  Pending: Rs.{pending:N0}");
            }
        }
    }

    public class FineService
    {
        public void IssueFine()
        {
            ConsoleHelper.Header("Issue Fine");
            string studentId = ConsoleHelper.ReadInput("Student ID");
            var student = DataStore.Students.FirstOrDefault(s => s.Id.Equals(studentId, StringComparison.OrdinalIgnoreCase));
            if (student == null) { ConsoleHelper.Error("Student not found."); return; }

            ConsoleHelper.PrintLine("Fine Type:");
            var types = Enum.GetValues<FineType>();
            for (int i = 0; i < types.Length; i++) ConsoleHelper.PrintLine($"  {i + 1}. {types[i]}");
            int c = ConsoleHelper.ReadMenuChoice(types.Length);
            if (c == 0) return;

            string amtStr = ConsoleHelper.ReadInput("Fine Amount (Rs.)");
            if (!decimal.TryParse(amtStr, out decimal amount) || amount <= 0)
            { ConsoleHelper.Error("Invalid amount."); return; }

            string reason = ConsoleHelper.ReadInput("Reason");

            var fine = new Fine(DataStore.NextFineId(), student.Id, student.Name, types[c - 1], amount, reason);
            DataStore.Fines.Add(fine);
            student.TotalFines += amount;
            DataStore.SaveAll();

            // Notify student
            var ns = new NotificationService();
            ns.Send(student.Id, "Fine Issued", $"A fine of Rs.{amount:N0} has been issued. Reason: {reason}");

            ConsoleHelper.Success($"Fine issued. ID: {fine.FineId}");
        }

        public void ViewAllFines()
        {
            ConsoleHelper.Header("All Fines");
            if (!DataStore.Fines.Any()) { ConsoleHelper.Warning("No fines."); return; }

            var headers = new[] { "Fine ID", "Student", "Type", "Amount", "Reason", "Paid?" };
            var rows = DataStore.Fines.Select(f => new[]
            {
                f.FineId, f.StudentName, f.Type.ToString(),
                $"Rs.{f.Amount:N0}", f.Reason, f.IsPaid ? "Yes" : "No"
            }).ToList();

            ConsoleHelper.Table(headers, rows);
        }

        public void MarkFinePaid()
        {
            ConsoleHelper.Header("Mark Fine as Paid");
            string fineId = ConsoleHelper.ReadInput("Fine ID");
            var fine = DataStore.Fines.FirstOrDefault(f => f.FineId.Equals(fineId, StringComparison.OrdinalIgnoreCase));
            if (fine == null) { ConsoleHelper.Error("Fine not found."); return; }
            if (fine.IsPaid) { ConsoleHelper.Warning("Already paid."); return; }

            fine.IsPaid = true;
            var student = DataStore.Students.FirstOrDefault(s => s.Id == fine.StudentId);
            if (student != null) student.TotalFines = Math.Max(0, student.TotalFines - fine.Amount);
            DataStore.SaveAll();
            ConsoleHelper.Success("Fine marked as paid.");
        }
    }
}
