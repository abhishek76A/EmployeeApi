using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System;
using Microsoft.AspNetCore.Mvc.Razor;

namespace EmployeeApi.Models
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public EmployeesController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // Get all employees
        [HttpGet("all")]
        public IActionResult GetEmployees()
        {
            var employees = new List<Employee>();
            string query = "SELECT Id, Name, Salary, Created_by, Created_ts, Modified_by, Modified_ts, Flag, Active, ts, ContactInfo, Department, Designation, Address, DateOfJoining, AssignedUsers, IsActive, Document_Link FROM Employees";

            using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        employees.Add(new Employee
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            Salary = reader.GetDecimal(2),
                            Created_by = reader.GetString(3),
                            Created_ts = reader.GetDateTime(4),
                            Modified_by = reader.IsDBNull(5) ? "admin" : reader.GetString(5),
                            Modified_ts = reader.IsDBNull(6) ? DateTime.UtcNow : reader.GetDateTime(6),
                            Flag = reader.GetString(7)[0],
                            Active = reader.GetString(8),
                            ts = reader.GetDateTime(9),
                            ContactInfo = reader.IsDBNull(10) ? 0 : reader.GetInt64(10),
                            Department = reader.IsDBNull(11) ? "Unknown" : reader.GetString(11),
                            Designation = reader.IsDBNull(12) ? "Unknown" : reader.GetString(12),
                            Address = reader.IsDBNull(13) ? "Unknown" : reader.GetString(13),
                            DateOfJoining = reader.IsDBNull(14) ? DateTime.UtcNow : reader.GetDateTime(14),
                            AssignedUsers = reader.IsDBNull(15) ? "None" : reader.GetString(15),
                            IsActive = reader.IsDBNull(16) ? true : reader.GetBoolean(16),
                            Document_Link = reader.IsDBNull(17) ? null : reader.GetString(17) // Read Document_Link
                        });
                    }
                }
            }

            return Ok(employees);
        }

        // Get employee by ID
        [HttpGet("details/{id}")]
        public IActionResult GetEmployeeById(int id)
        {
            Employee employee = null;
            string query = "SELECT Id, Name, Salary, Created_by, Created_ts, Modified_by, Modified_ts, Flag, Active, ts, ContactInfo, Department, Designation, Address, DateOfJoining, AssignedUsers, IsActive, Document_Link FROM Employees WHERE Id = @Id";

            using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Id", id);
                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        employee = new Employee
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            Salary = reader.GetDecimal(2),
                            Created_by = reader.GetString(3),
                            Created_ts = reader.GetDateTime(4),
                            Modified_by = reader.IsDBNull(5) ? "admin" : reader.GetString(5),
                            Modified_ts = reader.IsDBNull(6) ? DateTime.UtcNow : reader.GetDateTime(6),
                            Flag = reader.GetString(7)[0],
                            Active = reader.GetString(8),
                            ts = reader.GetDateTime(9),
                            ContactInfo = reader.IsDBNull(10) ? 0 : reader.GetInt64(10),
                            Department = reader.IsDBNull(11) ? "Unknown" : reader.GetString(11),
                            Designation = reader.IsDBNull(12) ? "Unknown" : reader.GetString(12),
                            Address = reader.IsDBNull(13) ? "Unknown" : reader.GetString(13),
                            DateOfJoining = reader.IsDBNull(14) ? DateTime.UtcNow : reader.GetDateTime(14),
                            AssignedUsers = reader.IsDBNull(15) ? "None" : reader.GetString(15),
                            IsActive = reader.IsDBNull(16) ? true : reader.GetBoolean(16),
                            Document_Link = reader.IsDBNull(17) ? null : reader.GetString(17) // Read Document_Link
                        };
                    }
                }
            }

            if (employee == null)
                return NotFound(new { message = "Employee not found" });

            return Ok(employee);
        }

        // Add a new employee
        [HttpPost("add")]
        public IActionResult AddEmployee([FromBody] Employee employee)
        {
            // Set default values for Modified_by and Modified_ts if they are not provided
            if (string.IsNullOrEmpty(employee.Modified_by))
            {
                employee.Modified_by = "admin"; // Set this to the actual logged-in user if possible
            }

            if (employee.Modified_ts == null)
            {
                employee.Modified_ts = DateTime.UtcNow; // Current timestamp
            }

            if (employee.ts == default)
            {
                employee.ts = DateTime.UtcNow;  // Set current timestamp if ts is not provided
            }

            // Provide default values for nullable fields
            if (string.IsNullOrEmpty(employee.Department)) employee.Department = "Unknown";
            if (string.IsNullOrEmpty(employee.Designation)) employee.Designation = "Unknown";
            if (string.IsNullOrEmpty(employee.Address)) employee.Address = "Unknown";
            if (string.IsNullOrEmpty(employee.AssignedUsers)) employee.AssignedUsers = "None";
            if (employee.ContactInfo == 0) employee.ContactInfo = 9999999999; // Default contact number
            if (employee.DateOfJoining == default) employee.DateOfJoining = DateTime.UtcNow;
            if (!employee.IsActive.HasValue) employee.IsActive = true; // Default to active status

            // Insert the employee data into the database, including the document link
            string query = "INSERT INTO Employees (Name, Salary, Created_by, Created_ts, Flag, Active, Modified_by, Modified_ts, ts, ContactInfo, Department, Designation, Address, DateOfJoining, AssignedUsers, IsActive, Document_Link) " +
                           "VALUES (@Name, @Salary, @Created_by, @Created_ts, @Flag, @Active, @Modified_by, @Modified_ts, @ts, @ContactInfo, @Department, @Designation, @Address, @DateOfJoining, @AssignedUsers, @IsActive, @Document_Link)";

            using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Name", employee.Name);
                command.Parameters.AddWithValue("@Salary", employee.Salary);
                command.Parameters.AddWithValue("@Created_by", employee.Created_by);
                command.Parameters.AddWithValue("@Created_ts", employee.Created_ts);
                command.Parameters.AddWithValue("@Flag", employee.Flag);
                command.Parameters.AddWithValue("@Active", employee.Active);
                command.Parameters.AddWithValue("@Modified_by", employee.Modified_by); // Default value "admin" will be used if not set
                command.Parameters.AddWithValue("@Modified_ts", employee.Modified_ts); // Default value will be set
                command.Parameters.AddWithValue("@ts", employee.ts);  // Add ts value
                command.Parameters.AddWithValue("@ContactInfo", employee.ContactInfo);  // Add ContactInfo value
                command.Parameters.AddWithValue("@Department", employee.Department);  // Add Department value
                command.Parameters.AddWithValue("@Designation", employee.Designation);  // Add Designation value
                command.Parameters.AddWithValue("@Address", employee.Address);  // Add Address value
                command.Parameters.AddWithValue("@DateOfJoining", employee.DateOfJoining);  // Add DateOfJoining value
                command.Parameters.AddWithValue("@AssignedUsers", employee.AssignedUsers);  // Add AssignedUsers value
                command.Parameters.AddWithValue("@IsActive", employee.IsActive);  // Add IsActive value
                command.Parameters.AddWithValue("@Document_Link", employee.Document_Link);  // Add Document_Link (SharePoint URL)

                connection.Open();
                command.ExecuteNonQuery();
            }

            return Ok(new { message = "Employee added successfully" });
        }

        // Delete an employee by ID
        [HttpDelete("remove/{id}")]
        public IActionResult DeleteEmployee(int id)
        {
            string query = "DELETE FROM Employees WHERE Id = @Id";

            using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Id", id);

                connection.Open();
                int rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected == 0)
                    return NotFound(new { message = "Employee not found" });
            }

            return Ok(new { message = "Employee deleted successfully" });
        }

        // Update an employee by ID
        [HttpPut("update/{id}")]
        public IActionResult UpdateEmployee(int id, [FromBody] Employee employee)
        {
            // Ensure required fields are set for Modified_by and Modified_ts
            if (string.IsNullOrEmpty(employee.Modified_by))
            {
                return BadRequest(new { message = "Modified_by field is required." });
            }

            if (employee.Modified_ts == null)
            {
                return BadRequest(new { message = "Modified_ts cannot be null." });
            }

            string updateQuery = "UPDATE Employees SET Name = @Name, Salary = @Salary, Modified_by = @Modified_by, Modified_ts = @Modified_ts, Flag = @Flag, Active = @Active, ts = @ts, ContactInfo = @ContactInfo, Department = @Department, Designation = @Designation, Address = @Address, DateOfJoining = @DateOfJoining, AssignedUsers = @AssignedUsers, IsActive = @IsActive, Document_Link = @Document_Link WHERE Id = @Id";

            using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                SqlCommand command = new SqlCommand(updateQuery, connection);
                command.Parameters.AddWithValue("@Id", id);
                command.Parameters.AddWithValue("@Name", employee.Name);
                command.Parameters.AddWithValue("@Salary", employee.Salary);
                command.Parameters.AddWithValue("@Modified_by", employee.Modified_by); // From request body
                command.Parameters.AddWithValue("@Modified_ts", employee.Modified_ts); // From request body
                command.Parameters.AddWithValue("@Flag", employee.Flag);
                command.Parameters.AddWithValue("@Active", employee.Active);
                command.Parameters.AddWithValue("@ts", employee.ts); // From request body
                command.Parameters.AddWithValue("@ContactInfo", employee.ContactInfo);
                command.Parameters.AddWithValue("@Department", employee.Department);
                command.Parameters.AddWithValue("@Designation", employee.Designation);
                command.Parameters.AddWithValue("@Address", employee.Address);
                command.Parameters.AddWithValue("@DateOfJoining", employee.DateOfJoining);
                command.Parameters.AddWithValue("@AssignedUsers", employee.AssignedUsers);
                command.Parameters.AddWithValue("@IsActive", employee.IsActive);
                command.Parameters.AddWithValue("@Document_Link", employee.Document_Link); // From request body

                connection.Open();
                int rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected == 0)
                    return NotFound(new { message = "Employee not found" });
            }

            return Ok(new { message = "Employee updated successfully" });
        }
    }
}
