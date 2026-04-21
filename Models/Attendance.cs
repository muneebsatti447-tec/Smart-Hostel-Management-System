namespace SmartHostel.Models
{
    public enum AttendanceStatus { Present, Absent, LateEntry }

    public class AttendanceRecord
    {
        public string RecordId { get; set; } = string.Empty;
        public string StudentId { get; set; } = string.Empty;
        public string StudentName { get; set; } = string.Empty;
        public DateTime Date { get; set; } = DateTime.Today;
        public AttendanceStatus Status { get; set; }
        public TimeSpan? EntryTime { get; set; }

        // Late entry threshold is 10:00 PM
        public static readonly TimeSpan LateEntryThreshold = new(22, 0, 0);

        public AttendanceRecord() { }

        public AttendanceRecord(string recordId, string studentId, string studentName,
                                AttendanceStatus status, TimeSpan? entryTime = null)
        {
            RecordId = recordId; StudentId = studentId;
            StudentName = studentName; Status = status; EntryTime = entryTime;
        }

        public bool IsLate => EntryTime.HasValue && EntryTime.Value > LateEntryThreshold;

        public override string ToString() =>
            $"{Date:dd-MMM-yyyy} | {StudentName} ({StudentId}) | {Status}" +
            (EntryTime.HasValue ? $" | Entry: {EntryTime:hh\\:mm}" : "");
    }

    public class Notification
    {
        public string NotificationId { get; set; } = string.Empty;
        public string RecipientId { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public Notification() { }

        public Notification(string id, string recipientId, string title, string message)
        {
            NotificationId = id; RecipientId = recipientId;
            Title = title; Message = message;
        }

        public override string ToString() =>
            $"[{(IsRead ? "READ" : "NEW")}] {CreatedAt:dd-MMM HH:mm} | {Title}";
    }
}
