namespace SmartHostel.Models
{
    public enum StaffRole { Cleaner, Electrician, Plumber, ITStaff, Guard, Manager }

    public class Staff : Person
    {
        public StaffRole Role { get; set; }
        public int AssignedComplaints { get; set; }
        public bool IsAvailable { get; set; } = true;

        public Staff() { }

        public Staff(string id, string name, string cnic, string phone, string email,
                     string gender, StaffRole role)
            : base(id, name, cnic, phone, email, gender)
        {
            Role = role;
        }

        public override string GetRole() => $"Staff ({Role})";
    }

    public class Warden : Person
    {
        public string Designation { get; set; } = "Warden";
        private readonly string _password;

        public Warden() { _password = "warden123"; }

        public Warden(string id, string name, string cnic, string phone, string email,
                      string gender, string password)
            : base(id, name, cnic, phone, email, gender)
        {
            _password = password;
            Designation = "Warden";
        }

        public bool Authenticate(string pwd) => _password == pwd;

        public override string GetRole() => Designation;
    }

    public class Admin : Person
    {
        private readonly string _password;

        public Admin() { _password = "admin123"; }

        public Admin(string id, string name, string phone, string email, string password)
            : base(id, name, "00000-0000000-0", phone, email, "N/A")
        {
            _password = password;
        }

        public bool Authenticate(string pwd) => _password == pwd;

        public override string GetRole() => "Admin";
    }
}
