# Smart Hostel Management System
## C# Console Application — Final Year Project

---

## 📁 Project Structure

```
SmartHostel/
├── Program.cs                    ← Entry point + all menus
├── SmartHostel.csproj            ← .NET 8 project file
├── PROJECT_REPORT.txt            ← Full academic report
│
├── Models/
│   ├── Person.cs                 ← Abstract base class
│   ├── Student.cs                ← Student entity
│   ├── Staff.cs                  ← Staff, Warden, Admin
│   ├── Room.cs                   ← Room entity
│   ├── Complaint.cs              ← Complaint entity
│   ├── Finance.cs                ← MessBill + Fine
│   └── Attendance.cs             ← AttendanceRecord + Notification
│
├── Interfaces/
│   ├── IReportable.cs
│   └── ICalculatable.cs
│
├── Services/
│   ├── AuthService.cs
│   ├── StudentService.cs
│   ├── RoomService.cs
│   ├── ComplaintService.cs
│   ├── MessFineService.cs
│   ├── AttendanceNotifService.cs
│   └── ReportService.cs
│
├── Utilities/
│   └── ConsoleHelper.cs          ← UI rendering + Validator
│
└── Data/
    └── DataStore.cs              ← Central data store + JSON I/O
```

---

## 🚀 How to Run

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)

### Steps

```bash
# 1. Navigate to project folder
cd SmartHostel

# 2. Build
dotnet build

# 3. Run
dotnet run
```

---

## 🔑 Default Login Credentials

| Role   | Username | Password   |
|--------|----------|------------|
| Admin  | ADM001   | admin123   |
| Warden | WRD001   | warden123  |

---

## 📦 Features at a Glance

| Module              | Features                                               |
|---------------------|--------------------------------------------------------|
| Authentication      | Role-based login (Admin / Warden), password masking    |
| Student Management  | Add, View, Update, Delete, Search (ID/CNIC/Name)       |
| Room Management     | Smart allocation, occupancy dashboard, filter/search   |
| Mess Management     | Bill generation, per-meal rates, payment tracking      |
| Complaint System    | Register, assign to staff, status lifecycle            |
| Fine System         | Manual + auto late-entry fines                         |
| Attendance          | Daily marking, auto-fine trigger, absentee reports     |
| Notifications       | Internal alert queue per user                          |
| Reports             | 7 report types, saved as .txt files                    |
| Data Persistence    | JSON file storage, loads on startup                    |

---

## 🎓 OOP Concepts Demonstrated

| Concept       | Where Used                                                  |
|---------------|-------------------------------------------------------------|
| Encapsulation | Private `_messDues`, `_password` fields with validation     |
| Inheritance   | `Person → Student, Staff, Warden`                           |
| Polymorphism  | `GetRole()` override, `CalculateFine()` per type            |
| Abstraction   | `IReportable`, `ICalculatable` interfaces; abstract `Person`|

---

## 📊 Sample Data Included

On first run, the system seeds:
- 4 Students (Ali Raza, Sara Khan, Ahmed Bilal, Fatima Noor)
- 8 Rooms (Singles, Doubles, Triples across Blocks A & B)
- 4 Staff members
- 4 Complaints (various statuses)
- 3 Mess bills
- 2 Fines

---

## 💾 Data Files

All data is automatically saved in:
```
Data/Saved/
  students.json
  rooms.json
  staff.json
  complaints.json
  messbills.json
  fines.json
  attendance.json
  notifications.json
```

Reports are saved in:
```
Reports/
  StudentReport_YYYYMMDD_HHMM.txt
  FinancialReport_YYYYMMDD_HHMM.txt
  ...
```
