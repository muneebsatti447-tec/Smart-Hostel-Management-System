using SmartHostel.Data;
using SmartHostel.Models;
using SmartHostel.Utilities;

namespace SmartHostel.Services
{
    public class ComplaintService
    {
        public void RegisterComplaint()
        {
            ConsoleHelper.Header("Register Complaint");
            string studentId = ConsoleHelper.ReadInput("Student ID");
            var student = DataStore.Students.FirstOrDefault(s => s.Id.Equals(studentId, StringComparison.OrdinalIgnoreCase));
            if (student == null) { ConsoleHelper.Error("Student not found."); return; }

            ConsoleHelper.PrintLine("Category:");
            var categories = Enum.GetValues<ComplaintCategory>();
            for (int i = 0; i < categories.Length; i++)
                ConsoleHelper.PrintLine($"  {i + 1}. {categories[i]}");

            int catChoice = ConsoleHelper.ReadMenuChoice(categories.Length);
            if (catChoice == 0) return;

            string desc = ConsoleHelper.ReadInput("Describe the issue");
            if (!Validator.IsNotEmpty(desc)) { ConsoleHelper.Error("Description cannot be empty."); return; }

            var complaint = new Complaint(
                DataStore.NextComplaintId(), student.Id, student.Name,
                student.RoomNumber, categories[catChoice - 1], desc
            );

            DataStore.Complaints.Add(complaint);
            student.ComplaintCount++;
            DataStore.SaveAll();

            // Notify warden
            var ns = new NotificationService();
            ns.Send("WRD001", "New Complaint",
                $"New {complaint.Category} complaint from {student.Name} (Room {student.RoomNumber}): {desc}");

            ConsoleHelper.Success($"Complaint filed. ID: {complaint.ComplaintId}");
        }

        public void ViewAllComplaints()
        {
            ConsoleHelper.Header("All Complaints");
            if (!DataStore.Complaints.Any()) { ConsoleHelper.Warning("No complaints."); return; }

            var headers = new[] { "ID", "Student", "Room", "Category", "Status", "Assigned To", "Date" };
            var rows = DataStore.Complaints.Select(c => new[]
            {
                c.ComplaintId, c.StudentName, c.RoomNumber,
                c.Category.ToString(), c.Status.ToString(),
                string.IsNullOrEmpty(c.AssignedStaffName) ? "Unassigned" : c.AssignedStaffName,
                c.DateFiled.ToString("dd-MMM-yy")
            }).ToList();

            ConsoleHelper.Table(headers, rows);
        }

        public void UpdateComplaintStatus()
        {
            ConsoleHelper.Header("Update Complaint Status");
            string id = ConsoleHelper.ReadInput("Complaint ID");
            var complaint = DataStore.Complaints.FirstOrDefault(c => c.ComplaintId.Equals(id, StringComparison.OrdinalIgnoreCase));
            if (complaint == null) { ConsoleHelper.Error("Complaint not found."); return; }

            ConsoleHelper.PrintLine($"Current Status: {complaint.Status}");
            ConsoleHelper.PrintLine("1. Pending");
            ConsoleHelper.PrintLine("2. In Progress");
            ConsoleHelper.PrintLine("3. Resolved");
            int c = ConsoleHelper.ReadMenuChoice(3);
            if (c == 0) return;

            complaint.Status = c switch { 1 => ComplaintStatus.Pending, 2 => ComplaintStatus.InProgress, _ => ComplaintStatus.Resolved };
            if (complaint.Status == ComplaintStatus.Resolved)
                complaint.DateResolved = DateTime.Now;

            DataStore.SaveAll();

            // Notify student
            var ns = new NotificationService();
            ns.Send(complaint.StudentId, "Complaint Update",
                $"Your complaint ({complaint.ComplaintId}) - {complaint.Category} - status updated to: {complaint.Status}");

            ConsoleHelper.Success("Complaint status updated.");
        }

        public void AssignToStaff()
        {
            ConsoleHelper.Header("Assign Complaint to Staff");
            string cmpId = ConsoleHelper.ReadInput("Complaint ID");
            var complaint = DataStore.Complaints.FirstOrDefault(c => c.ComplaintId.Equals(cmpId, StringComparison.OrdinalIgnoreCase));
            if (complaint == null) { ConsoleHelper.Error("Complaint not found."); return; }

            ConsoleHelper.PrintLine("Available Staff:");
            var availableStaff = DataStore.StaffMembers.Where(s => s.IsAvailable).ToList();
            if (!availableStaff.Any()) { ConsoleHelper.Warning("No available staff."); return; }

            for (int i = 0; i < availableStaff.Count; i++)
                ConsoleHelper.PrintLine($"  {i + 1}. {availableStaff[i].Name} ({availableStaff[i].Role})");

            int choice = ConsoleHelper.ReadMenuChoice(availableStaff.Count);
            if (choice == 0) return;

            var staff = availableStaff[choice - 1];
            complaint.AssignedStaffId   = staff.Id;
            complaint.AssignedStaffName = staff.Name;
            complaint.Status = ComplaintStatus.InProgress;
            staff.AssignedComplaints++;

            DataStore.SaveAll();
            ConsoleHelper.Success($"Complaint assigned to {staff.Name}.");
        }

        public void FilterComplaints()
        {
            ConsoleHelper.Header("Filter Complaints");
            ConsoleHelper.PrintLine("1. By Status");
            ConsoleHelper.PrintLine("2. By Category");
            ConsoleHelper.PrintLine("3. By Student ID");
            int c = ConsoleHelper.ReadMenuChoice(3);
            if (c == 0) return;

            List<Complaint> filtered = new();
            switch (c)
            {
                case 1:
                    ConsoleHelper.PrintLine("Status: 1=Pending 2=InProgress 3=Resolved");
                    int s = ConsoleHelper.ReadMenuChoice(3); if (s == 0) return;
                    var status = s switch { 1 => ComplaintStatus.Pending, 2 => ComplaintStatus.InProgress, _ => ComplaintStatus.Resolved };
                    filtered = DataStore.Complaints.Where(x => x.Status == status).ToList();
                    break;
                case 2:
                    var cats = Enum.GetValues<ComplaintCategory>();
                    for (int i = 0; i < cats.Length; i++) ConsoleHelper.PrintLine($"  {i + 1}. {cats[i]}");
                    int cat = ConsoleHelper.ReadMenuChoice(cats.Length); if (cat == 0) return;
                    filtered = DataStore.Complaints.Where(x => x.Category == cats[cat - 1]).ToList();
                    break;
                case 3:
                    string sid = ConsoleHelper.ReadInput("Student ID");
                    filtered = DataStore.Complaints.Where(x => x.StudentId.Equals(sid, StringComparison.OrdinalIgnoreCase)).ToList();
                    break;
            }

            if (!filtered.Any()) { ConsoleHelper.Warning("No complaints match."); return; }
            filtered.ForEach(x => ConsoleHelper.PrintLine(x.ToString()));
        }
    }
}
