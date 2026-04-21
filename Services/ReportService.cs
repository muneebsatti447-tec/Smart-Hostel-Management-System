using SmartHostel.Data;
using SmartHostel.Models;
using SmartHostel.Utilities;

namespace SmartHostel.Services
{
    public class ReportService
    {
        private static readonly string ReportsDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Reports");

        public void StudentReport()
        {
            ConsoleHelper.Header("Student Report");
            if (!DataStore.Students.Any()) { ConsoleHelper.Warning("No students."); return; }

            var sb = new System.Text.StringBuilder();
            sb.AppendLine("════════════════════════════════════════════════════════════════");
            sb.AppendLine("  SMART HOSTEL MANAGEMENT SYSTEM - STUDENT REPORT");
            sb.AppendLine($"  Generated: {DateTime.Now:dd-MMMM-yyyy HH:mm}");
            sb.AppendLine("════════════════════════════════════════════════════════════════");

            foreach (var s in DataStore.Students)
                sb.AppendLine(s.GenerateReport());

            sb.AppendLine($"\n  SUMMARY: {DataStore.Students.Count} students | " +
                          $"Active: {DataStore.Students.Count(s => s.Status == StudentStatus.Active)} | " +
                          $"Total Dues: Rs.{DataStore.Students.Sum(s => s.TotalDues()):N0}");

            string report = sb.ToString();
            Console.WriteLine(report);
            SaveReport("StudentReport", report);
        }

        public void RoomOccupancyReport()
        {
            ConsoleHelper.Header("Room Occupancy Report");
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("════════════════════════════════════════════════════════════════");
            sb.AppendLine("  ROOM OCCUPANCY REPORT");
            sb.AppendLine($"  Generated: {DateTime.Now:dd-MMMM-yyyy HH:mm}");
            sb.AppendLine("════════════════════════════════════════════════════════════════\n");

            foreach (var r in DataStore.Rooms)
                sb.AppendLine(r.GenerateReport());

            int total    = DataStore.Rooms.Count;
            int occupied = DataStore.Rooms.Count(r => r.Status == RoomStatus.Occupied);
            int avail    = DataStore.Rooms.Count(r => r.Status == RoomStatus.Available);
            int maint    = DataStore.Rooms.Count(r => r.Status == RoomStatus.UnderMaintenance);
            int beds     = DataStore.Rooms.Sum(r => r.Capacity);
            int usedBeds = DataStore.Rooms.Sum(r => r.OccupantIds.Count);

            sb.AppendLine($"  SUMMARY: {total} rooms | {occupied} occupied | {avail} available | {maint} maintenance");
            sb.AppendLine($"  Beds: {usedBeds}/{beds} occupied ({(beds > 0 ? usedBeds * 100.0 / beds : 0):F1}%)");

            string report = sb.ToString();
            Console.WriteLine(report);
            SaveReport("RoomOccupancyReport", report);
        }

        public void FinancialReport()
        {
            ConsoleHelper.Header("Financial Report");
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("════════════════════════════════════════════════════════════════");
            sb.AppendLine("  FINANCIAL REPORT");
            sb.AppendLine($"  Generated: {DateTime.Now:dd-MMMM-yyyy HH:mm}");
            sb.AppendLine("════════════════════════════════════════════════════════════════\n");

            // Mess revenue
            decimal totalMess     = DataStore.MessBills.Sum(b => b.TotalAmount);
            decimal collectedMess = DataStore.MessBills.Where(b => b.IsPaid).Sum(b => b.TotalAmount);
            decimal pendingMess   = totalMess - collectedMess;

            // Fines
            decimal totalFines    = DataStore.Fines.Sum(f => f.Amount);
            decimal paidFines     = DataStore.Fines.Where(f => f.IsPaid).Sum(f => f.Amount);
            decimal pendingFines  = totalFines - paidFines;

            // Room rent (all rooms x rent, simple monthly estimate)
            decimal roomRevenue = DataStore.Rooms.Where(r => r.OccupantIds.Count > 0)
                                           .Sum(r => r.MonthlyRent * r.OccupantIds.Count);

            sb.AppendLine("  ─── MESS FEES ───────────────────────────────────────────");
            sb.AppendLine($"  Total Billed  : Rs. {totalMess:N2}");
            sb.AppendLine($"  Collected     : Rs. {collectedMess:N2}");
            sb.AppendLine($"  Pending       : Rs. {pendingMess:N2}");
            sb.AppendLine();
            sb.AppendLine("  ─── FINES ────────────────────────────────────────────────");
            sb.AppendLine($"  Total Fines   : Rs. {totalFines:N2}");
            sb.AppendLine($"  Collected     : Rs. {paidFines:N2}");
            sb.AppendLine($"  Pending       : Rs. {pendingFines:N2}");
            sb.AppendLine();
            sb.AppendLine("  ─── ROOM RENT (Monthly Estimate) ─────────────────────────");
            sb.AppendLine($"  Estimated Rev : Rs. {roomRevenue:N2}");
            sb.AppendLine();
            sb.AppendLine("  ─── TOTAL FINANCIALS ────────────────────────────────────");
            sb.AppendLine($"  Total Income  : Rs. {(collectedMess + paidFines + roomRevenue):N2}");
            sb.AppendLine($"  Total Pending : Rs. {(pendingMess + pendingFines):N2}");

            string report = sb.ToString();
            Console.WriteLine(report);
            SaveReport("FinancialReport", report);
        }

        public void ComplaintSummaryReport()
        {
            ConsoleHelper.Header("Complaint Summary Report");
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("════════════════════════════════════════════════════════════════");
            sb.AppendLine("  COMPLAINT SUMMARY REPORT");
            sb.AppendLine($"  Generated: {DateTime.Now:dd-MMMM-yyyy HH:mm}");
            sb.AppendLine("════════════════════════════════════════════════════════════════\n");

            int total    = DataStore.Complaints.Count;
            int pending  = DataStore.Complaints.Count(c => c.Status == ComplaintStatus.Pending);
            int inprog   = DataStore.Complaints.Count(c => c.Status == ComplaintStatus.InProgress);
            int resolved = DataStore.Complaints.Count(c => c.Status == ComplaintStatus.Resolved);

            sb.AppendLine($"  Total      : {total}");
            sb.AppendLine($"  Pending    : {pending}");
            sb.AppendLine($"  In Progress: {inprog}");
            sb.AppendLine($"  Resolved   : {resolved}");
            sb.AppendLine();

            sb.AppendLine("  ─── BY CATEGORY ──────────────────────────────────────────");
            foreach (var cat in Enum.GetValues<ComplaintCategory>())
            {
                int cnt = DataStore.Complaints.Count(c => c.Category == cat);
                sb.AppendLine($"  {cat,-15}: {cnt}");
            }
            sb.AppendLine();
            sb.AppendLine("  ─── DETAILS ─────────────────────────────────────────────");
            DataStore.Complaints.ForEach(c => sb.AppendLine(c.GenerateReport()));

            string report = sb.ToString();
            Console.WriteLine(report);
            SaveReport("ComplaintSummaryReport", report);
        }

        public void ProblematicStudentReport()
        {
            ConsoleHelper.Header("Most Problematic Students");
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("════════════════════════════════════════════════════════════════");
            sb.AppendLine("  PROBLEMATIC STUDENT ANALYSIS");
            sb.AppendLine($"  Generated: {DateTime.Now:dd-MMMM-yyyy HH:mm}");
            sb.AppendLine("════════════════════════════════════════════════════════════════\n");

            // Score = complaints*3 + lateEntries*2 + (dues>3000 ? 5 : 0)
            var ranked = DataStore.Students
                .Select(s => new
                {
                    Student = s,
                    Score = s.ComplaintCount * 3 + s.LateEntries * 2 + (s.MessDues > 3000m ? 5 : 0) + s.TotalDaysAbsent
                })
                .OrderByDescending(x => x.Score)
                .ToList();

            sb.AppendLine($"  {"Rank",-5} {"Name",-20} {"Complaints",-12} {"Late",-6} {"Absent",-8} {"Dues",-12} {"Score",-6}");
            sb.AppendLine("  " + new string('-', 70));

            int rank = 1;
            foreach (var x in ranked)
            {
                sb.AppendLine($"  {rank,-5} {x.Student.Name,-20} {x.Student.ComplaintCount,-12} {x.Student.LateEntries,-6} {x.Student.TotalDaysAbsent,-8} Rs.{x.Student.MessDues,-10:N0} {x.Score,-6}");
                rank++;
            }

            string report = sb.ToString();
            Console.WriteLine(report);
            SaveReport("ProblematicStudentsReport", report);
        }

        public void ExpenseAnalytics()
        {
            ConsoleHelper.Header("Expense Analytics - Monthly Overview");
            var grouped = DataStore.MessBills
                .GroupBy(b => new { b.Year, b.Month })
                .OrderByDescending(g => g.Key.Year).ThenByDescending(g => g.Key.Month);

            var headers = new[] { "Month", "Bills", "Mess Total", "Collected", "Fines", "Grand Total" };
            var rows = grouped.Select(g =>
            {
                decimal messTotal = g.Sum(b => b.TotalAmount);
                decimal collected = g.Where(b => b.IsPaid).Sum(b => b.TotalAmount);
                decimal fines = DataStore.Fines
                    .Where(f => f.IssuedOn.Month == g.Key.Month && f.IssuedOn.Year == g.Key.Year)
                    .Sum(f => f.Amount);

                return new[]
                {
                    new DateTime(g.Key.Year, g.Key.Month, 1).ToString("MMM yyyy"),
                    g.Count().ToString(),
                    $"Rs.{messTotal:N0}", $"Rs.{collected:N0}",
                    $"Rs.{fines:N0}", $"Rs.{(collected + fines):N0}"
                };
            }).ToList();

            ConsoleHelper.Table(headers, rows);
        }

        private void SaveReport(string name, string content)
        {
            try
            {
                Directory.CreateDirectory(ReportsDir);
                string filename = Path.Combine(ReportsDir, $"{name}_{DateTime.Now:yyyyMMdd_HHmm}.txt");
                File.WriteAllText(filename, content);
                ConsoleHelper.Info($"Report saved: {filename}");
            }
            catch (Exception ex)
            {
                ConsoleHelper.Warning($"Could not save report: {ex.Message}");
            }
        }
    }
}
