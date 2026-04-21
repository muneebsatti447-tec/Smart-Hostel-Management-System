using SmartHostel.Interfaces;

namespace SmartHostel.Models
{
    public enum RoomType { Single, Double, Triple }
    public enum RoomStatus { Available, Occupied, UnderMaintenance }

    public class Room : IReportable, ICalculatable
    {
        public string RoomNumber { get; set; } = string.Empty;
        public RoomType Type { get; set; }
        public RoomStatus Status { get; set; } = RoomStatus.Available;
        public string Floor { get; set; } = string.Empty;
        public string Block { get; set; } = string.Empty;
        public string AllowedGender { get; set; } = "Any"; // Male / Female / Any
        public List<string> OccupantIds { get; set; } = new();
        private decimal _monthlyRent;

        public int Capacity => Type switch
        {
            RoomType.Single => 1,
            RoomType.Double => 2,
            RoomType.Triple => 3,
            _ => 1
        };

        public int AvailableSpots => Capacity - OccupantIds.Count;
        public bool IsFull => OccupantIds.Count >= Capacity;

        public decimal MonthlyRent
        {
            get => _monthlyRent;
            set => _monthlyRent = value > 0 ? value : throw new ArgumentException("Rent must be positive.");
        }

        public Room() { }

        public Room(string roomNumber, RoomType type, string floor, string block,
                    string allowedGender, decimal monthlyRent)
        {
            RoomNumber = roomNumber;
            Type = type;
            Floor = floor;
            Block = block;
            AllowedGender = allowedGender;
            MonthlyRent = monthlyRent;
        }

        public decimal CalculateFee() => _monthlyRent;

        // Polymorphism: fine differs per room type (damage charge)
        public decimal CalculateFine() => Type switch
        {
            RoomType.Single => 500m,
            RoomType.Double => 400m,
            RoomType.Triple => 300m,
            _ => 200m
        };

        public string GenerateReport()
        {
            return $"""
            ════════════════════════════════════════════════
             ROOM REPORT
            ════════════════════════════════════════════════
             Room No    : {RoomNumber}
             Type       : {Type}
             Block/Floor: {Block} / Floor {Floor}
             Status     : {Status}
             Gender     : {AllowedGender}
             Capacity   : {Capacity}  |  Occupied: {OccupantIds.Count}  |  Free: {AvailableSpots}
             Monthly Rent: Rs. {_monthlyRent:N2}
             Occupants  : {(OccupantIds.Count > 0 ? string.Join(", ", OccupantIds) : "None")}
            ════════════════════════════════════════════════
            """;
        }

        public override string ToString() =>
            $"Room {RoomNumber} [{Type}] Block {Block} Floor {Floor} | {Status} | Occupants: {OccupantIds.Count}/{Capacity} | Rent: Rs.{_monthlyRent:N0}";
    }
}
