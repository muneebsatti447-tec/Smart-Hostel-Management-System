using SmartHostel.Data;
using SmartHostel.Models;
using SmartHostel.Utilities;

namespace SmartHostel.Services
{
    public class StudentService
    {
        public void AddStudent()
        {
            ConsoleHelper.Header("Add New Student");

            string id = DataStore.NextStudentId();
            string name  = ConsoleHelper.ReadInput("Full Name");
            string cnic  = ConsoleHelper.ReadInput("CNIC (XXXXX-XXXXXXX-X)");
            string phone = ConsoleHelper.ReadInput("Phone (0XXX-XXXXXXX)");
            string email = ConsoleHelper.ReadInput("Email");
            string gender= ConsoleHelper.ReadInput("Gender (Male/Female)");
            string dept  = ConsoleHelper.ReadInput("Department");
            string sem   = ConsoleHelper.ReadInput("Semester");

            // Validation
            if (!Validator.IsNotEmpty(name))       { ConsoleHelper.Error("Name is required."); return; }
            if (!Validator.IsValidCNIC(cnic))      { ConsoleHelper.Error("Invalid CNIC format. Use: XXXXX-XXXXXXX-X"); return; }
            if (!Validator.IsValidPhone(phone))    { ConsoleHelper.Error("Invalid phone. Use: 0XXX-XXXXXXX"); return; }
            if (!Validator.IsValidEmail(email))    { ConsoleHelper.Error("Invalid email."); return; }
            if (!Validator.IsValidGender(gender))  { ConsoleHelper.Error("Gender must be Male or Female."); return; }
            if (!Validator.IsNotEmpty(dept))       { ConsoleHelper.Error("Department is required."); return; }
            if (!Validator.IsNotEmpty(sem))        { ConsoleHelper.Error("Semester is required."); return; }

            if (DataStore.Students.Any(s => s.CNIC == cnic))
            { ConsoleHelper.Error("A student with this CNIC already exists."); return; }

            var student = new Student(id, name, cnic, phone, email, gender, dept, sem);
            DataStore.Students.Add(student);
            DataStore.SaveAll();

            ConsoleHelper.Success($"Student '{name}' added successfully with ID: {id}");

            // Optionally allocate room right away
            if (ConsoleHelper.Confirm("Would you like to allocate a room now?"))
            {
                var roomSvc = new RoomService();
                roomSvc.AllocateRoom(student);
            }
        }

        public void ViewAllStudents()
        {
            ConsoleHelper.Header("All Students");

            if (!DataStore.Students.Any()) { ConsoleHelper.Warning("No students found."); return; }

            var headers = new[] { "ID", "Name", "Gender", "Dept", "Room", "Dues (Rs.)", "Status" };
            var rows = DataStore.Students.Select(s => new[]
            {
                s.Id, s.Name, s.Gender, s.Department,
                string.IsNullOrEmpty(s.RoomNumber) ? "N/A" : s.RoomNumber,
                s.TotalDues().ToString("N0"), s.Status.ToString()
            }).ToList();

            ConsoleHelper.Table(headers, rows);
            ConsoleHelper.PrintLine($"Total: {DataStore.Students.Count} student(s)");
        }

        public void UpdateStudent()
        {
            ConsoleHelper.Header("Update Student");
            string id = ConsoleHelper.ReadInput("Enter Student ID");
            var s = Find(id);
            if (s == null) return;

            ConsoleHelper.PrintLine($"Editing: {s.Name}  (leave blank to keep current)");

            string name  = ConsoleHelper.ReadInput($"Name [{s.Name}]");
            string phone = ConsoleHelper.ReadInput($"Phone [{s.Phone}]");
            string email = ConsoleHelper.ReadInput($"Email [{s.Email}]");
            string dept  = ConsoleHelper.ReadInput($"Department [{s.Department}]");
            string sem   = ConsoleHelper.ReadInput($"Semester [{s.Semester}]");

            if (!string.IsNullOrWhiteSpace(name))  s.Name  = name;
            if (!string.IsNullOrWhiteSpace(phone) && Validator.IsValidPhone(phone)) s.Phone = phone;
            if (!string.IsNullOrWhiteSpace(email) && Validator.IsValidEmail(email)) s.Email = email;
            if (!string.IsNullOrWhiteSpace(dept))  s.Department = dept;
            if (!string.IsNullOrWhiteSpace(sem))   s.Semester   = sem;

            DataStore.SaveAll();
            ConsoleHelper.Success("Student updated successfully.");
        }

        public void DeleteStudent()
        {
            ConsoleHelper.Header("Delete Student");
            string id = ConsoleHelper.ReadInput("Enter Student ID to delete");
            var s = Find(id);
            if (s == null) return;

            ConsoleHelper.Warning($"You are about to delete: {s.Name} ({s.Id})");
            if (!ConsoleHelper.Confirm("Confirm deletion?")) { ConsoleHelper.Info("Deletion cancelled."); return; }

            // Vacate room
            if (!string.IsNullOrEmpty(s.RoomNumber))
            {
                var room = DataStore.Rooms.FirstOrDefault(r => r.RoomNumber == s.RoomNumber);
                room?.OccupantIds.Remove(s.Id);
                if (room != null && room.OccupantIds.Count == 0)
                    room.Status = RoomStatus.Available;
            }

            DataStore.Students.Remove(s);
            DataStore.SaveAll();
            ConsoleHelper.Success($"Student '{s.Name}' deleted.");
        }

        public void SearchStudent()
        {
            ConsoleHelper.Header("Search Student");
            ConsoleHelper.PrintLine("1. Search by ID");
            ConsoleHelper.PrintLine("2. Search by CNIC");
            ConsoleHelper.PrintLine("3. Search by Name");
            int choice = ConsoleHelper.ReadMenuChoice(3);
            if (choice == 0) return;

            string term = ConsoleHelper.ReadInput("Enter search term");
            List<Student> results = choice switch
            {
                1 => DataStore.Students.Where(s => s.Id.Equals(term, StringComparison.OrdinalIgnoreCase)).ToList(),
                2 => DataStore.Students.Where(s => s.CNIC.Equals(term, StringComparison.OrdinalIgnoreCase)).ToList(),
                3 => DataStore.Students.Where(s => s.Name.Contains(term, StringComparison.OrdinalIgnoreCase)).ToList(),
                _ => new()
            };

            if (!results.Any()) { ConsoleHelper.Warning("No students found."); return; }
            foreach (var s in results)
                Console.WriteLine(s.GenerateReport());
        }

        public void ViewStudentProfile()
        {
            ConsoleHelper.Header("Student Profile");
            string id = ConsoleHelper.ReadInput("Enter Student ID");
            var s = Find(id);
            if (s == null) return;
            Console.WriteLine(s.GenerateReport());

            // Show complaints
            var cmps = DataStore.Complaints.Where(c => c.StudentId == id).ToList();
            if (cmps.Any())
            {
                ConsoleHelper.PrintLine("\nComplaints:");
                cmps.ForEach(c => ConsoleHelper.PrintLine($"  {c}"));
            }
        }

        public Student? Find(string id, bool silent = false)
        {
            var s = DataStore.Students.FirstOrDefault(x => x.Id.Equals(id, StringComparison.OrdinalIgnoreCase));
            if (s == null && !silent) ConsoleHelper.Error($"Student with ID '{id}' not found.");
            return s;
        }
    }
}
