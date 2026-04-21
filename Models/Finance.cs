using SmartHostel.Interfaces;

namespace SmartHostel.Models
{
    public class MessBill : IReportable
    {
        public string BillId { get; set; } = string.Empty;
        public string StudentId { get; set; } = string.Empty;
        public string StudentName { get; set; } = string.Empty;
        public int Month { get; set; }
        public int Year { get; set; }
        public decimal BaseFee { get; set; } = 4500m;
        public int MealsConsumed { get; set; }
        public decimal PerMealRate { get; set; } = 150m;
        public decimal Penalty { get; set; }
        public bool IsPaid { get; set; }
        public DateTime GeneratedOn { get; set; } = DateTime.Now;

        public decimal TotalAmount => (MealsConsumed * PerMealRate) + Penalty;

        public MessBill() { }

        public MessBill(string billId, string studentId, string studentName, int month, int year)
        {
            BillId = billId; StudentId = studentId; StudentName = studentName;
            Month = month; Year = year;
        }

        public string GenerateReport()
        {
            return $"""
            ════════════════════════════════════════════════
             MESS INVOICE
            ════════════════════════════════════════════════
             Bill ID    : {BillId}
             Student    : {StudentName} ({StudentId})
             Period     : {new DateTime(Year, Month, 1):MMMM yyyy}
             Meals      : {MealsConsumed} meals @ Rs.{PerMealRate}/meal
             Penalty    : Rs. {Penalty:N2}
             ──────────────────────────────────────────────
             TOTAL      : Rs. {TotalAmount:N2}
             Status     : {(IsPaid ? "✓ PAID" : "✗ UNPAID")}
             Generated  : {GeneratedOn:dd-MMM-yyyy}
            ════════════════════════════════════════════════
            """;
        }
    }

    public enum FineType { LateEntry, MessDues, RoomDamage, PropertyDamage, Other }

    public class Fine : IReportable
    {
        public string FineId { get; set; } = string.Empty;
        public string StudentId { get; set; } = string.Empty;
        public string StudentName { get; set; } = string.Empty;
        public FineType Type { get; set; }
        public decimal Amount { get; set; }
        public string Reason { get; set; } = string.Empty;
        public bool IsPaid { get; set; }
        public DateTime IssuedOn { get; set; } = DateTime.Now;

        public Fine() { }

        public Fine(string fineId, string studentId, string studentName, FineType type, decimal amount, string reason)
        {
            FineId = fineId; StudentId = studentId; StudentName = studentName;
            Type = type; Amount = amount; Reason = reason;
        }

        public string GenerateReport()
        {
            return $"""
            ════════════════════════════════════════════════
             FINE RECORD
            ════════════════════════════════════════════════
             Fine ID    : {FineId}
             Student    : {StudentName} ({StudentId})
             Type       : {Type}
             Amount     : Rs. {Amount:N2}
             Reason     : {Reason}
             Status     : {(IsPaid ? "✓ PAID" : "✗ UNPAID")}
             Issued On  : {IssuedOn:dd-MMM-yyyy}
            ════════════════════════════════════════════════
            """;
        }
    }
}
