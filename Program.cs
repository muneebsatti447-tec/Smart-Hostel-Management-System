using SmartHostel.Data;
using SmartHostel.Services;
using SmartHostel.Utilities;

namespace SmartHostel
{
    class Program
    {
        private static readonly AuthService       _auth       = new();
        private static readonly StudentService    _students   = new();
        private static readonly RoomService       _rooms      = new();
        private static readonly ComplaintService  _complaints = new();
        private static readonly MessService       _mess       = new();
        private static readonly FineService       _fines      = new();
        private static readonly AttendanceService _attendance = new();
        private static readonly NotificationService _notifs   = new();
        private static readonly ReportService     _reports    = new();

        static void Main(string[] args)
        {
            // ── Startup ────────────────────────────────────────────────────
            ConsoleHelper.Banner();
            ConsoleHelper.Info("Loading data...");
            DataStore.LoadAll();
            DataStore.SeedData();
            ConsoleHelper.Success("System ready.\n");
            ConsoleHelper.Pause();

            // ── Authentication loop ────────────────────────────────────────
            while (true)
            {
                ConsoleHelper.Banner();
                bool loggedIn = _auth.Login();
                if (!loggedIn) { ConsoleHelper.Pause(); continue; }

                // ── Role-based menu ────────────────────────────────────────
                if (_auth.CurrentRole == UserRole.Admin)
                    AdminMenu();
                else if (_auth.CurrentRole == UserRole.Warden)
                    WardenMenu();

                _auth.Logout();
                ConsoleHelper.Pause();

                ConsoleHelper.Banner();
                ConsoleHelper.PrintLine("1. Login again");
                ConsoleHelper.PrintLine("0. Exit");
                int c = ConsoleHelper.ReadMenuChoice(1);
                if (c == 0) break;
            }

            DataStore.SaveAll();
            ConsoleHelper.Success("Data saved. Goodbye!");
        }

        // ══════════════════════════════════════════════════════════════════
        //  ADMIN MENU
        // ══════════════════════════════════════════════════════════════════
        static void AdminMenu()
        {
            while (true)
            {
                ConsoleHelper.Banner();
                Console.ForegroundColor = ConsoleColor.Cyan;
                ConsoleHelper.PrintLine("  ╔═══════════════════════════════╗");
                ConsoleHelper.PrintLine("  ║       ADMIN MAIN MENU         ║");
                ConsoleHelper.PrintLine("  ╠═══════════════════════════════╣");
                ConsoleHelper.PrintLine("  ║  1. Student Management        ║");
                ConsoleHelper.PrintLine("  ║  2. Room Management           ║");
                ConsoleHelper.PrintLine("  ║  3. Mess Management           ║");
                ConsoleHelper.PrintLine("  ║  4. Complaint Management      ║");
                ConsoleHelper.PrintLine("  ║  5. Fine Management           ║");
                ConsoleHelper.PrintLine("  ║  6. Attendance                ║");
                ConsoleHelper.PrintLine("  ║  7. Reports & Analytics       ║");
                ConsoleHelper.PrintLine("  ║  8. Notifications             ║");
                ConsoleHelper.PrintLine("  ║  0. Logout                    ║");
                ConsoleHelper.PrintLine("  ╚═══════════════════════════════╝");
                Console.ResetColor();

                int choice = ConsoleHelper.ReadMenuChoice(8);
                switch (choice)
                {
                    case 0: return;
                    case 1: StudentMenu(); break;
                    case 2: RoomMenu(); break;
                    case 3: MessMenu(); break;
                    case 4: ComplaintMenu(); break;
                    case 5: FineMenu(); break;
                    case 6: AttendanceMenu(); break;
                    case 7: ReportMenu(); break;
                    case 8: _notifs.ViewNotifications("ADM001"); ConsoleHelper.Pause(); break;
                }
            }
        }

        // ══════════════════════════════════════════════════════════════════
        //  WARDEN MENU
        // ══════════════════════════════════════════════════════════════════
        static void WardenMenu()
        {
            while (true)
            {
                ConsoleHelper.Banner();
                Console.ForegroundColor = ConsoleColor.Magenta;
                ConsoleHelper.PrintLine("  ╔═══════════════════════════════╗");
                ConsoleHelper.PrintLine("  ║      WARDEN MAIN MENU         ║");
                ConsoleHelper.PrintLine("  ╠═══════════════════════════════╣");
                ConsoleHelper.PrintLine("  ║  1. View All Students         ║");
                ConsoleHelper.PrintLine("  ║  2. Room Occupancy Dashboard  ║");
                ConsoleHelper.PrintLine("  ║  3. Manage Complaints         ║");
                ConsoleHelper.PrintLine("  ║  4. Mark Attendance           ║");
                ConsoleHelper.PrintLine("  ║  5. Issue Fine                ║");
                ConsoleHelper.PrintLine("  ║  6. Generate Mess Bill        ║");
                ConsoleHelper.PrintLine("  ║  7. View Reports              ║");
                ConsoleHelper.PrintLine("  ║  8. Notifications             ║");
                ConsoleHelper.PrintLine("  ║  0. Logout                    ║");
                ConsoleHelper.PrintLine("  ╚═══════════════════════════════╝");
                Console.ResetColor();

                int choice = ConsoleHelper.ReadMenuChoice(8);
                switch (choice)
                {
                    case 0: return;
                    case 1: _students.ViewAllStudents(); ConsoleHelper.Pause(); break;
                    case 2: _rooms.OccupancyDashboard(); ConsoleHelper.Pause(); break;
                    case 3: ComplaintMenu(); break;
                    case 4: _attendance.MarkAttendance(); ConsoleHelper.Pause(); break;
                    case 5: _fines.IssueFine(); ConsoleHelper.Pause(); break;
                    case 6: _mess.GenerateBill(); ConsoleHelper.Pause(); break;
                    case 7: ReportMenu(); break;
                    case 8: _notifs.ViewNotifications("WRD001"); ConsoleHelper.Pause(); break;
                }
            }
        }

        // ══════════════════════════════════════════════════════════════════
        //  SUB-MENUS
        // ══════════════════════════════════════════════════════════════════
        static void StudentMenu()
        {
            while (true)
            {
                ConsoleHelper.Banner();
                ConsoleHelper.Header("Student Management");
                ConsoleHelper.PrintLine("  1. Add New Student");
                ConsoleHelper.PrintLine("  2. View All Students");
                ConsoleHelper.PrintLine("  3. Update Student");
                ConsoleHelper.PrintLine("  4. Delete Student");
                ConsoleHelper.PrintLine("  5. Search Student");
                ConsoleHelper.PrintLine("  6. View Student Profile");
                ConsoleHelper.PrintLine("  0. Back");

                int c = ConsoleHelper.ReadMenuChoice(6);
                switch (c)
                {
                    case 0: return;
                    case 1: _students.AddStudent();          ConsoleHelper.Pause(); break;
                    case 2: _students.ViewAllStudents();     ConsoleHelper.Pause(); break;
                    case 3: _students.UpdateStudent();       ConsoleHelper.Pause(); break;
                    case 4: _students.DeleteStudent();       ConsoleHelper.Pause(); break;
                    case 5: _students.SearchStudent();       ConsoleHelper.Pause(); break;
                    case 6: _students.ViewStudentProfile();  ConsoleHelper.Pause(); break;
                }
            }
        }

        static void RoomMenu()
        {
            while (true)
            {
                ConsoleHelper.Banner();
                ConsoleHelper.Header("Room Management");
                ConsoleHelper.PrintLine("  1. Add Room");
                ConsoleHelper.PrintLine("  2. View All Rooms");
                ConsoleHelper.PrintLine("  3. Occupancy Dashboard");
                ConsoleHelper.PrintLine("  4. Allocate Room to Student");
                ConsoleHelper.PrintLine("  5. Room Change Request");
                ConsoleHelper.PrintLine("  6. Set Room Status");
                ConsoleHelper.PrintLine("  7. Filter Rooms");
                ConsoleHelper.PrintLine("  0. Back");

                int c = ConsoleHelper.ReadMenuChoice(7);
                switch (c)
                {
                    case 0: return;
                    case 1: _rooms.AddRoom();               ConsoleHelper.Pause(); break;
                    case 2: _rooms.ViewAllRooms();          ConsoleHelper.Pause(); break;
                    case 3: _rooms.OccupancyDashboard();    ConsoleHelper.Pause(); break;
                    case 4:
                        string sid = ConsoleHelper.ReadInput("Student ID to allocate room");
                        var st = _students.Find(sid);
                        if (st != null) _rooms.AllocateRoom(st);
                        ConsoleHelper.Pause();
                        break;
                    case 5: _rooms.RoomChangeRequest();     ConsoleHelper.Pause(); break;
                    case 6: _rooms.SetRoomStatus();         ConsoleHelper.Pause(); break;
                    case 7: _rooms.FilterRooms();           ConsoleHelper.Pause(); break;
                }
            }
        }

        static void MessMenu()
        {
            while (true)
            {
                ConsoleHelper.Banner();
                ConsoleHelper.Header("Mess Management");
                ConsoleHelper.PrintLine("  1. Generate Mess Bill");
                ConsoleHelper.PrintLine("  2. View All Bills");
                ConsoleHelper.PrintLine("  3. Mark Bill as Paid");
                ConsoleHelper.PrintLine("  4. Monthly Revenue Summary");
                ConsoleHelper.PrintLine("  0. Back");

                int c = ConsoleHelper.ReadMenuChoice(4);
                switch (c)
                {
                    case 0: return;
                    case 1: _mess.GenerateBill();       ConsoleHelper.Pause(); break;
                    case 2: _mess.ViewAllBills();       ConsoleHelper.Pause(); break;
                    case 3: _mess.MarkAsPaid();         ConsoleHelper.Pause(); break;
                    case 4: _mess.MonthlyRevenue();     ConsoleHelper.Pause(); break;
                }
            }
        }

        static void ComplaintMenu()
        {
            while (true)
            {
                ConsoleHelper.Banner();
                ConsoleHelper.Header("Complaint Management");
                ConsoleHelper.PrintLine("  1. Register Complaint");
                ConsoleHelper.PrintLine("  2. View All Complaints");
                ConsoleHelper.PrintLine("  3. Update Complaint Status");
                ConsoleHelper.PrintLine("  4. Assign Complaint to Staff");
                ConsoleHelper.PrintLine("  5. Filter Complaints");
                ConsoleHelper.PrintLine("  0. Back");

                int c = ConsoleHelper.ReadMenuChoice(5);
                switch (c)
                {
                    case 0: return;
                    case 1: _complaints.RegisterComplaint();      ConsoleHelper.Pause(); break;
                    case 2: _complaints.ViewAllComplaints();      ConsoleHelper.Pause(); break;
                    case 3: _complaints.UpdateComplaintStatus();  ConsoleHelper.Pause(); break;
                    case 4: _complaints.AssignToStaff();          ConsoleHelper.Pause(); break;
                    case 5: _complaints.FilterComplaints();       ConsoleHelper.Pause(); break;
                }
            }
        }

        static void FineMenu()
        {
            while (true)
            {
                ConsoleHelper.Banner();
                ConsoleHelper.Header("Fine Management");
                ConsoleHelper.PrintLine("  1. Issue Fine");
                ConsoleHelper.PrintLine("  2. View All Fines");
                ConsoleHelper.PrintLine("  3. Mark Fine as Paid");
                ConsoleHelper.PrintLine("  0. Back");

                int c = ConsoleHelper.ReadMenuChoice(3);
                switch (c)
                {
                    case 0: return;
                    case 1: _fines.IssueFine();      ConsoleHelper.Pause(); break;
                    case 2: _fines.ViewAllFines();   ConsoleHelper.Pause(); break;
                    case 3: _fines.MarkFinePaid();   ConsoleHelper.Pause(); break;
                }
            }
        }

        static void AttendanceMenu()
        {
            while (true)
            {
                ConsoleHelper.Banner();
                ConsoleHelper.Header("Attendance Management");
                ConsoleHelper.PrintLine("  1. Mark Today's Attendance");
                ConsoleHelper.PrintLine("  2. View Attendance Reports");
                ConsoleHelper.PrintLine("  0. Back");

                int c = ConsoleHelper.ReadMenuChoice(2);
                switch (c)
                {
                    case 0: return;
                    case 1: _attendance.MarkAttendance();    ConsoleHelper.Pause(); break;
                    case 2: _attendance.AttendanceReport();  ConsoleHelper.Pause(); break;
                }
            }
        }

        static void ReportMenu()
        {
            while (true)
            {
                ConsoleHelper.Banner();
                ConsoleHelper.Header("Reports & Analytics");
                ConsoleHelper.PrintLine("  1. Student Report");
                ConsoleHelper.PrintLine("  2. Room Occupancy Report");
                ConsoleHelper.PrintLine("  3. Financial Report");
                ConsoleHelper.PrintLine("  4. Complaint Summary Report");
                ConsoleHelper.PrintLine("  5. Attendance Report");
                ConsoleHelper.PrintLine("  6. Most Problematic Students");
                ConsoleHelper.PrintLine("  7. Expense Analytics (Monthly)");
                ConsoleHelper.PrintLine("  0. Back");

                int c = ConsoleHelper.ReadMenuChoice(7);
                switch (c)
                {
                    case 0: return;
                    case 1: _reports.StudentReport();            ConsoleHelper.Pause(); break;
                    case 2: _reports.RoomOccupancyReport();      ConsoleHelper.Pause(); break;
                    case 3: _reports.FinancialReport();          ConsoleHelper.Pause(); break;
                    case 4: _reports.ComplaintSummaryReport();   ConsoleHelper.Pause(); break;
                    case 5: _attendance.AttendanceReport();      ConsoleHelper.Pause(); break;
                    case 6: _reports.ProblematicStudentReport(); ConsoleHelper.Pause(); break;
                    case 7: _reports.ExpenseAnalytics();         ConsoleHelper.Pause(); break;
                }
            }
        }
    }
}
