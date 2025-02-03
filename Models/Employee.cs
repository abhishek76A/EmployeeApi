namespace EmployeeApi.Models
{
    public class Employee
    {
        // Nullable field for timestamp (TS) if needed in the future
        internal DateTime? TS;

        // Primary Key for the employee
        public int Id { get; set; }

        // Employee's name
        public string Name { get; set; }

        // Employee's salary
        public decimal Salary { get; set; }

        // Name of the person who created the record
        public string Created_by { get; set; }

        // Timestamp when the record was created
        public DateTime Created_ts { get; set; }

        // Name of the person who last modified the record
        public string Modified_by { get; set; }

        // Timestamp when the record was last modified (nullable)
        public DateTime? Modified_ts { get; set; }

        // Flag indicating the employee status (e.g., 'C' for current)
        public char Flag { get; set; }

        // Employee's active status, stored as a string ("True" or "False")
        public string Active { get; set; }

        // Timestamp for the employee's custom tracking (non-nullable DateTime)
        public DateTime ts { get; set; }

        // Employee's contact information (bigint for phone numbers or other contacts)
        public long ContactInfo { get; set; }

        // Employee's department (nullable if not available)
        public string Department { get; set; }

        // Employee's designation/job title (nullable if not available)
        public string Designation { get; set; }

        // Employee's address (nullable if not available)
        public string Address { get; set; }

        // Date of employee joining (nullable)
        public DateTime? DateOfJoining { get; set; }

        // List of assigned users to this employee (nullable if not available)
        public string AssignedUsers { get; set; }

        // Indicates if the employee is active (nullable)
        public bool? IsActive { get; set; }

        // The link to the supporting document uploaded to SharePoint
        public string Document_Link { get; set; }  // New property for the document link
    }
}
