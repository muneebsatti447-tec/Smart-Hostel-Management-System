using System.Text.Json;
using SmartHostel.Models;

namespace SmartHostel.Data
{
    public static class DataStore
    {
        // ── Core collections ──────────────────────────────────────────────
        public static List<Student>           Students        { get; set; } = new();
        public static List<Room>              Rooms           { get; set; } = new();
        public static List<Staff>             StaffMembers    { get; set; } = new();
        public static List<Complaint>         Complaints      { get; set; } = new();
        public static List<MessBill>          MessBills       { get; set; } = new();
        public static List<Fine>              Fines           { get; set; } = new();
        public static List<AttendanceRecord>  Attendance      { get; set; } = new();
        public static List<Notification>      Notifications   { get; set; } = new();

        // ── Auth ──────────────────────────────────────────────────────────
        public static Admin   AdminUser   { get; set; } = new("ADM001", "System Admin", "0300-0000000", "admin@hostel.pk", "admin123");
        public static Warden  WardenUser  { get; set; } = new("WRD001", "Dr. Khalid Mehmood", "42301-1234567-1", "0321-1234567", "warden@hostel.pk", "Male", "warden123");

        // ── File paths ────────────────────────────────────────────────────
        private static readonly string DataDir  = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "Saved");
        private static string F(string name) => Path.Combine(DataDir, name);

        private static readonly JsonSerializerOptions _opts = new() { WriteIndented = true };

        // ── Save ──────────────────────────────────────────────────────────
        public static void SaveAll()
        {
            Directory.CreateDirectory(DataDir);
            File.WriteAllText(F("students.json"),    JsonSerializer.Serialize(Students, _opts));
            File.WriteAllText(F("rooms.json"),       JsonSerializer.Serialize(Rooms, _opts));
            File.WriteAllText(F("staff.json"),       JsonSerializer.Serialize(StaffMembers, _opts));
            File.WriteAllText(F("complaints.json"),  JsonSerializer.Serialize(Complaints, _opts));
            File.WriteAllText(F("messbills.json"),   JsonSerializer.Serialize(MessBills, _opts));
            File.WriteAllText(F("fines.json"),       JsonSerializer.Serialize(Fines, _opts));
            File.WriteAllText(F("attendance.json"),  JsonSerializer.Serialize(Attendance, _opts));
            File.WriteAllText(F("notifications.json"),JsonSerializer.Serialize(Notifications, _opts));
        }

        // ── Load ──────────────────────────────────────────────────────────
        public static void LoadAll()
        {
            Students = TryLoad<Student>(F("students.json"));
            Rooms = TryLoad<Room>(F("rooms.json"));
            StaffMembers = TryLoad<Staff>(F("staff.json"));
            Complaints = TryLoad<Complaint>(F("complaints.json"));
            MessBills = TryLoad<MessBill>(F("messbills.json"));
            Fines = TryLoad<Fine>(F("fines.json"));
            Attendance = TryLoad<AttendanceRecord>(F("attendance.json"));
            Notifications = TryLoad<Notification>(F("notifications.json"));
        }

        private static List<T> TryLoad<T>(string path)
        {
            try
            {
                if (File.Exists(path))
                    return JsonSerializer.Deserialize<List<T>>(File.ReadAllText(path), _opts) ?? new();
            }
            catch { }

            return new List<T>();
        }

        // ── ID generators ─────────────────────────────────────────────────
        public static string NextStudentId()    => $"STU{(Students.Count + 1):D3}";
        public static string NextComplaintId()  => $"CMP{(Complaints.Count + 1):D3}";
        public static string NextBillId()       => $"BIL{(MessBills.Count + 1):D3}";
        public static string NextFineId()       => $"FIN{(Fines.Count + 1):D3}";
        public static string NextAttendId()     => $"ATT{(Attendance.Count + 1):D4}";
        public static string NextNotifId()      => $"NTF{(Notifications.Count + 1):D4}";

        // ── Seed sample data ───────────────────────────────────────────────
        public static void SeedData()
        {
            if (Students.Count > 0) return; // Already seeded

            // Rooms
            Rooms.AddRange(new[]
            {
                new Room("A101", RoomType.Single, "1", "A", "Male",   8000),
                new Room("A102", RoomType.Double, "1", "A", "Male",   6000),
                new Room("A103", RoomType.Triple, "1", "A", "Male",   4500),
                new Room("B101", RoomType.Single, "1", "B", "Female", 8000),
                new Room("B102", RoomType.Double, "1", "B", "Female", 6000),
                new Room("B103", RoomType.Triple, "1", "B", "Female", 4500),
                new Room("A201", RoomType.Double, "2", "A", "Male",   6000),
                new Room("A202", RoomType.Single, "2", "A", "Male",   8000) { Status = RoomStatus.UnderMaintenance },
            });

            // Students
            var s1 = new Student("STU001","Ali Raza","42101-1234567-1","0300-1111111","ali@uni.pk","Male","CS","5th");
            s1.RoomNumber = "A102"; s1.MessDues = 4500; s1.LateEntries = 2; s1.ComplaintCount = 1;
            s1.TotalDaysPresent = 25; s1.TotalDaysAbsent = 5;
            Rooms.First(r => r.RoomNumber == "A102").OccupantIds.Add("STU001");

            var s2 = new Student("STU002","Sara Khan","42201-2345678-2","0311-2222222","sara@uni.pk","Female","EE","3rd");
            s2.RoomNumber = "B102"; s2.MessDues = 2000; s2.LateEntries = 0; s2.ComplaintCount = 2;
            s2.TotalDaysPresent = 28; s2.TotalDaysAbsent = 2;
            Rooms.First(r => r.RoomNumber == "B102").OccupantIds.Add("STU002");

            var s3 = new Student("STU003","Ahmed Bilal","42301-3456789-3","0333-3333333","ahmed@uni.pk","Male","ME","1st");
            s3.RoomNumber = "A103"; s3.MessDues = 9000; s3.LateEntries = 5; s3.ComplaintCount = 4;
            s3.TotalDaysPresent = 18; s3.TotalDaysAbsent = 12;
            Rooms.First(r => r.RoomNumber == "A103").OccupantIds.Add("STU003");

            var s4 = new Student("STU004","Fatima Noor","42401-4567890-4","0345-4444444","fatima@uni.pk","Female","BBA","7th");
            s4.RoomNumber = "B103"; s4.MessDues = 1500; s4.LateEntries = 1;
            s4.TotalDaysPresent = 29; s4.TotalDaysAbsent = 1;
            Rooms.First(r => r.RoomNumber == "B103").OccupantIds.Add("STU004");

            Students.AddRange(new[] { s1, s2, s3, s4 });

            // Update room statuses
            foreach (var r in Rooms)
                if (r.OccupantIds.Count > 0 && r.Status == RoomStatus.Available)
                    r.Status = r.IsFull ? RoomStatus.Occupied : RoomStatus.Available;

            // Staff
            StaffMembers.AddRange(new[]
            {
                new Staff("STA001","Imran Electrician","11111-1111111-1","0300-1010101","imran@hostel.pk","Male",StaffRole.Electrician),
                new Staff("STA002","Khalid Plumber",   "22222-2222222-2","0311-2020202","khalid@hostel.pk","Male",StaffRole.Plumber),
                new Staff("STA003","Meena Cleaner",    "33333-3333333-3","0333-3030303","meena@hostel.pk","Female",StaffRole.Cleaner),
                new Staff("STA004","Tariq IT",         "44444-4444444-4","0345-4040404","tariq@hostel.pk","Male",StaffRole.ITStaff),
            });

            // Complaints
            Complaints.AddRange(new[]
            {
                new Complaint("CMP001","STU001","Ali Raza","A102",ComplaintCategory.Electricity,"Light not working in room") { Status = ComplaintStatus.InProgress, AssignedStaffId = "STA001", AssignedStaffName = "Imran Electrician" },
                new Complaint("CMP002","STU002","Sara Khan","B102",ComplaintCategory.Water,"No hot water") { Status = ComplaintStatus.Pending },
                new Complaint("CMP003","STU002","Sara Khan","B102",ComplaintCategory.Internet,"WiFi disconnects frequently") { Status = ComplaintStatus.Resolved, AssignedStaffId = "STA004", AssignedStaffName = "Tariq IT", DateResolved = DateTime.Now.AddDays(-3) },
                new Complaint("CMP004","STU003","Ahmed Bilal","A103",ComplaintCategory.Cleanliness,"Bathroom not cleaned") { Status = ComplaintStatus.Pending },
            });

            // Mess bills
            MessBills.AddRange(new[]
            {
                new MessBill("BIL001","STU001","Ali Raza",DateTime.Now.Month,DateTime.Now.Year) { MealsConsumed = 30, IsPaid = false },
                new MessBill("BIL002","STU002","Sara Khan",DateTime.Now.Month,DateTime.Now.Year) { MealsConsumed = 28, IsPaid = true },
                new MessBill("BIL003","STU003","Ahmed Bilal",DateTime.Now.Month,DateTime.Now.Year) { MealsConsumed = 20, Penalty = 500, IsPaid = false },
            });

            // Fines
            Fines.AddRange(new[]
            {
                new Fine("FIN001","STU003","Ahmed Bilal",FineType.LateEntry,200,"Returned after 10 PM on 3 occasions"),
                new Fine("FIN002","STU003","Ahmed Bilal",FineType.MessDues,500,"Outstanding mess dues over 30 days"),
            });

            SaveAll();
        }
    }
}
