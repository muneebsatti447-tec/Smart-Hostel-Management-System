using SmartHostel.Data;
using SmartHostel.Models;
using SmartHostel.Utilities;

namespace SmartHostel.Services
{
    public class RoomService
    {
        public void AddRoom()
        {
            ConsoleHelper.Header("Add New Room");
            string num   = ConsoleHelper.ReadInput("Room Number (e.g. A301)");
            string block = ConsoleHelper.ReadInput("Block (A/B/C)");
            string floor = ConsoleHelper.ReadInput("Floor Number");

            ConsoleHelper.PrintLine("Room Type: 1=Single  2=Double  3=Triple");
            int typeChoice = ConsoleHelper.ReadMenuChoice(3);
            if (typeChoice == 0) return;
            RoomType type = (RoomType)(typeChoice - 1);

            string gender  = ConsoleHelper.ReadInput("Allowed Gender (Male/Female/Any)");
            string rentStr = ConsoleHelper.ReadInput("Monthly Rent (Rs.)");

            if (DataStore.Rooms.Any(r => r.RoomNumber.Equals(num, StringComparison.OrdinalIgnoreCase)))
            { ConsoleHelper.Error("Room with this number already exists."); return; }

            if (!decimal.TryParse(rentStr, out decimal rent) || rent <= 0)
            { ConsoleHelper.Error("Invalid rent amount."); return; }

            var room = new Room(num, type, floor, block, gender, rent);
            DataStore.Rooms.Add(room);
            DataStore.SaveAll();
            ConsoleHelper.Success($"Room {num} added successfully.");
        }

        public void ViewAllRooms()
        {
            ConsoleHelper.Header("All Rooms");
            if (!DataStore.Rooms.Any()) { ConsoleHelper.Warning("No rooms found."); return; }

            var headers = new[] { "Room No", "Type", "Block", "Floor", "Gender", "Status", "Occupancy", "Rent (Rs.)" };
            var rows = DataStore.Rooms.Select(r => new[]
            {
                r.RoomNumber, r.Type.ToString(), r.Block, r.Floor,
                r.AllowedGender, r.Status.ToString(),
                $"{r.OccupantIds.Count}/{r.Capacity}",
                r.MonthlyRent.ToString("N0")
            }).ToList();

            ConsoleHelper.Table(headers, rows);
        }

        public void OccupancyDashboard()
        {
            ConsoleHelper.Header("Room Occupancy Dashboard");
            int total   = DataStore.Rooms.Count;
            int occupied  = DataStore.Rooms.Count(r => r.Status == RoomStatus.Occupied);
            int available = DataStore.Rooms.Count(r => r.Status == RoomStatus.Available);
            int maint     = DataStore.Rooms.Count(r => r.Status == RoomStatus.UnderMaintenance);
            int partFull  = DataStore.Rooms.Count(r => r.OccupantIds.Count > 0 && !r.IsFull && r.Status == RoomStatus.Available);

            int totalCapacity = DataStore.Rooms.Sum(r => r.Capacity);
            int totalOccupied = DataStore.Rooms.Sum(r => r.OccupantIds.Count);

            ConsoleHelper.PrintLine();
            ConsoleHelper.PrintLine($"  Total Rooms       : {total}");
            ConsoleHelper.PrintLine($"  Total Capacity    : {totalCapacity} beds");
            ConsoleHelper.PrintLine($"  Currently Occupied: {totalOccupied} beds");
            ConsoleHelper.PrintLine($"  Occupancy Rate    : {(totalCapacity > 0 ? (totalOccupied * 100.0 / totalCapacity):0):F1}%");
            ConsoleHelper.PrintLine();
            Console.ForegroundColor = ConsoleColor.Green;
            ConsoleHelper.PrintLine($"  ✓ Available       : {available} room(s)  (incl. {partFull} partially filled)");
            Console.ForegroundColor = ConsoleColor.Red;
            ConsoleHelper.PrintLine($"  ✗ Fully Occupied  : {occupied} room(s)");
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            ConsoleHelper.PrintLine($"  ⚠ Under Maint.    : {maint} room(s)");
            Console.ResetColor();
        }

        // Smart Room Allocation Algorithm
        public void AllocateRoom(Student student)
        {
            ConsoleHelper.Header("Smart Room Allocation");

            // Filter: status Available, gender matches, has free spot
            var candidates = DataStore.Rooms.Where(r =>
                r.Status != RoomStatus.UnderMaintenance &&
                !r.IsFull &&
                (r.AllowedGender.Equals("Any", StringComparison.OrdinalIgnoreCase) ||
                 r.AllowedGender.Equals(student.Gender, StringComparison.OrdinalIgnoreCase))
            ).OrderBy(r => r.AvailableSpots).ThenBy(r => r.MonthlyRent).ToList();

            if (!candidates.Any())
            {
                ConsoleHelper.Error($"No suitable room available for {student.Gender} student.");
                return;
            }

            ConsoleHelper.PrintLine("Available rooms (sorted by best fit):");
            ConsoleHelper.PrintLine();

            for (int i = 0; i < candidates.Count; i++)
                ConsoleHelper.PrintLine($"  {i + 1}. {candidates[i]}");

            ConsoleHelper.PrintLine();
            string choice = ConsoleHelper.ReadInput($"Select room number [1-{candidates.Count}] or 'auto' for best fit");

            Room selected;
            if (choice.Equals("auto", StringComparison.OrdinalIgnoreCase))
            {
                selected = candidates.First();
            }
            else if (int.TryParse(choice, out int idx) && idx >= 1 && idx <= candidates.Count)
            {
                selected = candidates[idx - 1];
            }
            else { ConsoleHelper.Error("Invalid selection."); return; }

            // Vacate previous room if any
            if (!string.IsNullOrEmpty(student.RoomNumber))
            {
                var prev = DataStore.Rooms.FirstOrDefault(r => r.RoomNumber == student.RoomNumber);
                prev?.OccupantIds.Remove(student.Id);
                if (prev != null && prev.OccupantIds.Count == 0)
                    prev.Status = RoomStatus.Available;
            }

            selected.OccupantIds.Add(student.Id);
            selected.Status = selected.IsFull ? RoomStatus.Occupied : RoomStatus.Available;
            student.RoomNumber = selected.RoomNumber;

            DataStore.SaveAll();

            // Send notification
            var notifSvc = new NotificationService();
            notifSvc.Send(student.Id, "Room Allocated",
                $"You have been allocated Room {selected.RoomNumber} in Block {selected.Block}. Monthly rent: Rs.{selected.MonthlyRent:N0}");

            ConsoleHelper.Success($"Room {selected.RoomNumber} allocated to {student.Name}.");
        }

        public void RoomChangeRequest()
        {
            ConsoleHelper.Header("Room Change Request");
            string studentId = ConsoleHelper.ReadInput("Student ID");
            var student = DataStore.Students.FirstOrDefault(s => s.Id.Equals(studentId, StringComparison.OrdinalIgnoreCase));
            if (student == null) { ConsoleHelper.Error("Student not found."); return; }

            ConsoleHelper.PrintLine($"Current Room: {(string.IsNullOrEmpty(student.RoomNumber) ? "Not Assigned" : student.RoomNumber)}");
            string reason = ConsoleHelper.ReadInput("Reason for room change");

            // Create a notification/request for warden
            var notifSvc = new NotificationService();
            notifSvc.Send("WRD001", "Room Change Request",
                $"Student {student.Name} ({student.Id}) from Room {student.RoomNumber} requests room change. Reason: {reason}");

            ConsoleHelper.Success("Room change request submitted. Warden will process it.");
        }

        public void SetRoomStatus()
        {
            ConsoleHelper.Header("Set Room Status");
            string num = ConsoleHelper.ReadInput("Room Number");
            var room = DataStore.Rooms.FirstOrDefault(r => r.RoomNumber.Equals(num, StringComparison.OrdinalIgnoreCase));
            if (room == null) { ConsoleHelper.Error("Room not found."); return; }

            ConsoleHelper.PrintLine("1. Available");
            ConsoleHelper.PrintLine("2. Occupied");
            ConsoleHelper.PrintLine("3. Under Maintenance");
            int c = ConsoleHelper.ReadMenuChoice(3);
            if (c == 0) return;

            room.Status = c switch { 1 => RoomStatus.Available, 2 => RoomStatus.Occupied, _ => RoomStatus.UnderMaintenance };
            DataStore.SaveAll();
            ConsoleHelper.Success($"Room {num} status set to {room.Status}.");
        }

        public void FilterRooms()
        {
            ConsoleHelper.Header("Filter Rooms");
            ConsoleHelper.PrintLine("1. By Type (Single/Double/Triple)");
            ConsoleHelper.PrintLine("2. By Status");
            ConsoleHelper.PrintLine("3. By Gender");
            int c = ConsoleHelper.ReadMenuChoice(3);
            if (c == 0) return;

            List<Room> filtered = new();
            switch (c)
            {
                case 1:
                    ConsoleHelper.PrintLine("Type: 1=Single 2=Double 3=Triple");
                    int t = ConsoleHelper.ReadMenuChoice(3); if (t == 0) return;
                    filtered = DataStore.Rooms.Where(r => r.Type == (RoomType)(t - 1)).ToList();
                    break;
                case 2:
                    ConsoleHelper.PrintLine("Status: 1=Available 2=Occupied 3=UnderMaintenance");
                    int st = ConsoleHelper.ReadMenuChoice(3); if (st == 0) return;
                    RoomStatus status = st switch { 1 => RoomStatus.Available, 2 => RoomStatus.Occupied, _ => RoomStatus.UnderMaintenance };
                    filtered = DataStore.Rooms.Where(r => r.Status == status).ToList();
                    break;
                case 3:
                    string g = ConsoleHelper.ReadInput("Gender (Male/Female/Any)");
                    filtered = DataStore.Rooms.Where(r => r.AllowedGender.Equals(g, StringComparison.OrdinalIgnoreCase)).ToList();
                    break;
            }

            if (!filtered.Any()) { ConsoleHelper.Warning("No rooms match the filter."); return; }
            foreach (var r in filtered) ConsoleHelper.PrintLine(r.ToString());
        }
    }
}
