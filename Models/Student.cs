using SmartHostel.Interfaces;

namespace SmartHostel.Models
{
    public enum StudentStatus { Active, Inactive, Suspended }

    public class Student : Person, IReportable, ICalculatable
    {
        public string RoomNumber { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public string Semester { get; set; } = string.Empty;
        public StudentStatus Status { get; set; } = StudentStatus.Active;

        // Financial
        private decimal _messDues;
        private decimal _totalFines;

        public decimal MessDues
        {
            get => _messDues;
            set => _messDues = value >= 0 ? value : throw new ArgumentException("Mess dues cannot be negative.");
        }

        public decimal TotalFines
        {
            get => _totalFines;
            set => _totalFines = value >= 0 ? value : throw new ArgumentException("Fines cannot be negative.");
        }

        // Attendance
        public int TotalDaysPresent { get; set; }
        public int TotalDaysAbsent { get; set; }
        public int LateEntries { get; set; }
        public int ComplaintCount { get; set; }

        public Student() { }

        public Student(string id, string name, string cnic, string phone, string email,
                       string gender, string department, string semester)
            : base(id, name, cnic, phone, email, gender)
        {
            Department = department;
            Semester = semester;
        }

        public override string GetRole() => "Student";

        public decimal CalculateFee()
        {
            // Base fee depends on room type (resolved via RoomService)
            return _messDues;
        }

        public decimal CalculateFine()
        {
            decimal lateFine = LateEntries * 100m;      // Rs.100 per late entry
            decimal messPenalty = _messDues > 3000m ? _messDues * 0.05m : 0m;
            return _totalFines + lateFine + messPenalty;
        }

        public decimal TotalDues() => _messDues + CalculateFine();

        public string GenerateReport()
        {
            return $"""
            ════════════════════════════════════════════════
             STUDENT REPORT
            ════════════════════════════════════════════════
             Name       : {Name}
             ID         : {Id}
             CNIC       : {CNIC}
             Gender     : {Gender}
             Department : {Department}
             Semester   : {Semester}
             Room       : {(string.IsNullOrEmpty(RoomNumber) ? "Not Assigned" : RoomNumber)}
             Status     : {Status}
             Mess Dues  : Rs. {_messDues:N2}
             Total Fines: Rs. {CalculateFine():N2}
             Total Dues : Rs. {TotalDues():N2}
             Complaints : {ComplaintCount}
             Late Entries: {LateEntries}
             Attendance : {TotalDaysPresent} present / {TotalDaysAbsent} absent
            ════════════════════════════════════════════════
            """;
        }
    }
}
