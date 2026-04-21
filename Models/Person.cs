using System.Text.Json.Serialization;

namespace SmartHostel.Models
{
    public abstract class Person
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string CNIC { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public DateTime DateAdded { get; set; } = DateTime.Now;

        protected Person() { }
        protected Person(string id, string name, string cnic, string phone, string email, string gender)
        {
            Id = id; Name = name; CNIC = cnic;
            Phone = phone; Email = email; Gender = gender;
        }

        public abstract string GetRole();

        public override string ToString() =>
            $"[{GetRole()}] {Name} (ID: {Id}) | CNIC: {CNIC} | Phone: {Phone}";
    }
}
