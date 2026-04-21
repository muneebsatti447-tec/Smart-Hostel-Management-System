using SmartHostel.Interfaces;

namespace SmartHostel.Models
{
    public enum ComplaintCategory { Electricity, Water, Internet, Cleanliness, Noise, Other }
    public enum ComplaintStatus { Pending, InProgress, Resolved }

    public class Complaint : IReportable
    {
        public string ComplaintId { get; set; } = string.Empty;
        public string StudentId { get; set; } = string.Empty;
        public string StudentName { get; set; } = string.Empty;
        public string RoomNumber { get; set; } = string.Empty;
        public ComplaintCategory Category { get; set; }
        public ComplaintStatus Status { get; set; } = ComplaintStatus.Pending;
        public string Description { get; set; } = string.Empty;
        public string AssignedStaffId { get; set; } = string.Empty;
        public string AssignedStaffName { get; set; } = string.Empty;
        public DateTime DateFiled { get; set; } = DateTime.Now;
        public DateTime? DateResolved { get; set; }

        public Complaint() { }

        public Complaint(string id, string studentId, string studentName, string room,
                         ComplaintCategory category, string description)
        {
            ComplaintId = id;
            StudentId = studentId;
            StudentName = studentName;
            RoomNumber = room;
            Category = category;
            Description = description;
        }

        public string GenerateReport()
        {
            return $"""
            ════════════════════════════════════════════════
             COMPLAINT REPORT
            ════════════════════════════════════════════════
             ID         : {ComplaintId}
             Student    : {StudentName} ({StudentId})
             Room       : {RoomNumber}
             Category   : {Category}
             Status     : {Status}
             Description: {Description}
             Assigned To: {(string.IsNullOrEmpty(AssignedStaffName) ? "Unassigned" : AssignedStaffName)}
             Date Filed : {DateFiled:dd-MMM-yyyy HH:mm}
             Resolved   : {(DateResolved.HasValue ? DateResolved.Value.ToString("dd-MMM-yyyy HH:mm") : "Pending")}
            ════════════════════════════════════════════════
            """;
        }

        public override string ToString() =>
            $"[{ComplaintId}] {Category} | {StudentName} (Room {RoomNumber}) | {Status} | {DateFiled:dd-MMM-yyyy}";
    }
}
