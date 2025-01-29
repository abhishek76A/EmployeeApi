namespace EmployeeApi.Models
{
    public class Employee
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Salary { get; set; }
        public string Created_by { get; set; }
        public DateTime Created_ts { get; set; }
        public string Modified_by { get; set; }
        public DateTime? Modified_ts { get; set; } // Nullable, because it may not be set for new records
        public char Flag { get; set; }
        public string Active { get; set; } // This can hold "True" or "False" as a string
    }
}
