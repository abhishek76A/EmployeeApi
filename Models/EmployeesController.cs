using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System;

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
            string query = "SELECT Id, Name, Salary, Created_by, Created_ts, Modified_by, Modified_ts, Flag, Active FROM Employees";

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
                            Modified_by = reader.IsDBNull(5) ? null : reader.GetString(5),
                            Modified_ts = reader.IsDBNull(6) ? (DateTime?)null : reader.GetDateTime(6),
                            Flag = reader.GetString(7)[0],
                            Active = reader.GetString(8)
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
            string query = "SELECT Id, Name, Salary, Created_by, Created_ts, Modified_by, Modified_ts, Flag, Active FROM Employees WHERE Id = @Id";

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
                            Modified_by = reader.IsDBNull(5) ? null : reader.GetString(5),
                            Modified_ts = reader.IsDBNull(6) ? (DateTime?)null : reader.GetDateTime(6),
                            Flag = reader.GetString(7)[0],
                            Active = reader.GetString(8)
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
            string query = "INSERT INTO Employees (Name, Salary, Created_by, Created_ts, Flag, Active) VALUES (@Name, @Salary, @Created_by, @Created_ts, @Flag, @Active)";

            using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Name", employee.Name);
                command.Parameters.AddWithValue("@Salary", employee.Salary);
                command.Parameters.AddWithValue("@Created_by", employee.Created_by);
                command.Parameters.AddWithValue("@Created_ts", employee.Created_ts);
                command.Parameters.AddWithValue("@Flag", employee.Flag);
                command.Parameters.AddWithValue("@Active", employee.Active);

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
            string updateQuery = "UPDATE Employees SET Name = @Name, Salary = @Salary, Modified_by = @Modified_by, Modified_ts = @Modified_ts, Flag = @Flag, Active = @Active WHERE Id = @Id";

            using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                SqlCommand command = new SqlCommand(updateQuery, connection);
                command.Parameters.AddWithValue("@Id", id);
                command.Parameters.AddWithValue("@Name", employee.Name);
                command.Parameters.AddWithValue("@Salary", employee.Salary);
                command.Parameters.AddWithValue("@Modified_by", employee.Modified_by);
                command.Parameters.AddWithValue("@Modified_ts", employee.Modified_ts);
                command.Parameters.AddWithValue("@Flag", employee.Flag);
                command.Parameters.AddWithValue("@Active", employee.Active);

                connection.Open();
                int rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected == 0)
                    return NotFound(new { message = "Employee not found" });
            }

            return Ok(new { message = "Employee updated successfully" });
        }
    }
}
