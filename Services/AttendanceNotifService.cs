using SmartHostel.Data;
using SmartHostel.Models;
using SmartHostel.Utilities;

namespace SmartHostel.Services
{
    public class AttendanceService
    {
        public void MarkAttendance()
        {
            ConsoleHelper.Header("Mark Daily Attendance");
            ConsoleHelper.PrintLine($"Date: {DateTime.Today:dd-MMMM-yyyy}");
            ConsoleHelper.PrintLine();

            var active = DataStore.Students.Where(s => s.Status == StudentStatus.Active).ToList();
            if (!active.Any()) { ConsoleHelper.Warning("No active students."); return; }

            int presentCount = 0, absentCount = 0, lateCount = 0;
            bool autoFineIssued = false;

            foreach (var student in active)
            {
                ConsoleHelper.PrintLine($"  Student: {student.Name} ({student.Id}) | Room: {student.RoomNumber}");
                ConsoleHelper.PrintLine("  1=Present  2=Absent  3=Late Entry");
                int c = ConsoleHelper.ReadMenuChoice(3);
                if (c == 0) continue;

                AttendanceStatus status = c switch { 1 => AttendanceStatus.Present, 2 => AttendanceStatus.Absent, _ => AttendanceStatus.LateEntry };
                TimeSpan? entryTime = null;

                if (status == AttendanceStatus.LateEntry)
                {
                    string entryStr = ConsoleHelper.ReadInput("Entry time (HH:MM, 24-hr)");
                    if (TimeSpan.TryParse(entryStr, out TimeSpan t)) entryTime = t;
                    student.LateEntries++;

                    // Auto fine after 3 late entries in current month
                    if (student.LateEntries % 3 == 0)
                    {
                        var fine = new Fine(DataStore.NextFineId(), student.Id, student.Name,
                            FineType.LateEntry, 300m, $"Accumulated {student.LateEntries} late entries");
                        DataStore.Fines.Add(fine);
                        student.TotalFines += 300m;
                        autoFineIssued = true;
                        new NotificationService().Send(student.Id, "Auto Fine",
                            $"Fine of Rs.300 issued for {student.LateEntries} late entries.");
                    }
                }

                var record = new AttendanceRecord(
                    DataStore.NextAttendId(), student.Id, student.Name, status, entryTime
                );
                DataStore.Attendance.Add(record);

                switch (status)
                {
                    case AttendanceStatus.Present:  student.TotalDaysPresent++; presentCount++; break;
                    case AttendanceStatus.Absent:   student.TotalDaysAbsent++;  absentCount++;  break;
                    case AttendanceStatus.LateEntry: student.TotalDaysPresent++; lateCount++;   break;
                }
                ConsoleHelper.PrintLine();
            }

            DataStore.SaveAll();
            ConsoleHelper.Success($"Attendance marked: {presentCount} Present | {absentCount} Absent | {lateCount} Late");
            if (autoFineIssued) ConsoleHelper.Warning("Auto late-entry fines were issued to applicable students.");
        }

        public void AttendanceReport()
        {
            ConsoleHelper.Header("Attendance Report");
            ConsoleHelper.PrintLine("1. View Today's Attendance");
            ConsoleHelper.PrintLine("2. View Student Attendance Summary");
            ConsoleHelper.PrintLine("3. Absentee Report (students absent > 5 days)");
            int c = ConsoleHelper.ReadMenuChoice(3);
            if (c == 0) return;

            switch (c)
            {
                case 1:
                    var today = DataStore.Attendance.Where(a => a.Date.Date == DateTime.Today).ToList();
                    if (!today.Any()) { ConsoleHelper.Warning("No attendance for today."); return; }
                    today.ForEach(a => ConsoleHelper.PrintLine(a.ToString()));
                    break;
                case 2:
                    var headers = new[] { "ID", "Name", "Present", "Absent", "Late", "Attend %" };
                    var rows = DataStore.Students.Select(s => new[]
                    {
                        s.Id, s.Name, s.TotalDaysPresent.ToString(), s.TotalDaysAbsent.ToString(),
                        s.LateEntries.ToString(),
                        (s.TotalDaysPresent + s.TotalDaysAbsent) > 0
                            ? $"{s.TotalDaysPresent * 100.0 / (s.TotalDaysPresent + s.TotalDaysAbsent):F1}%"
                            : "N/A"
                    }).ToList();
                    ConsoleHelper.Table(headers, rows);
                    break;
                case 3:
                    var absentees = DataStore.Students.Where(s => s.TotalDaysAbsent > 5).ToList();
                    if (!absentees.Any()) { ConsoleHelper.Warning("No students absent more than 5 days."); return; }
                    ConsoleHelper.PrintLine("HIGH ABSENTEES:");
                    absentees.ForEach(s => ConsoleHelper.PrintLine($"  {s.Name} ({s.Id}) | Absent: {s.TotalDaysAbsent} days | Room: {s.RoomNumber}"));
                    break;
            }
        }
    }

    public class NotificationService
    {
        public void Send(string recipientId, string title, string message)
        {
            var notif = new Notification(DataStore.NextNotifId(), recipientId, title, message);
            DataStore.Notifications.Add(notif);
        }

        public void ViewNotifications(string recipientId)
        {
            ConsoleHelper.Header("Notifications");
            var notifs = DataStore.Notifications
                .Where(n => n.RecipientId.Equals(recipientId, StringComparison.OrdinalIgnoreCase))
                .OrderByDescending(n => n.CreatedAt)
                .ToList();

            if (!notifs.Any()) { ConsoleHelper.Warning("No notifications."); return; }

            foreach (var n in notifs)
            {
                Console.ForegroundColor = n.IsRead ? ConsoleColor.DarkGray : ConsoleColor.White;
                ConsoleHelper.PrintLine($"[{(n.IsRead ? "READ" : "NEW")}] {n.CreatedAt:dd-MMM HH:mm} | {n.Title}");
                ConsoleHelper.PrintLine($"       {n.Message}");
                n.IsRead = true;
            }
            Console.ResetColor();
            DataStore.SaveAll();
        }
    }
}
