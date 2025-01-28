using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;

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
        [HttpGet]
        public IActionResult GetEmployees()
        {
            var employees = new List<Employee>();
            string query = "SELECT Id, Name, Salary FROM Employees";

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
                            Salary = reader.GetDecimal(2)
                        });
                    }
                }
            }

            return Ok(employees);
        }

        // Get employee by ID
        [HttpGet("{id}")]
        public IActionResult GetEmployeeById(int id)
        {
            Employee employee = null;
            string query = "SELECT Id, Name, Salary FROM Employees WHERE Id = @Id";

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
                            Salary = reader.GetDecimal(2)
                        };
                    }
                }
            }

            if (employee == null)
                return NotFound(new { message = "Employee not found" });

            return Ok(employee);
        }

        // Add a new employee
        [HttpPost]
        public IActionResult AddEmployee([FromBody] Employee employee)
        {
            string query = "INSERT INTO Employees (Name, Salary) VALUES (@Name, @Salary)";

            using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Name", employee.Name);
                command.Parameters.AddWithValue("@Salary", employee.Salary);

                connection.Open();
                command.ExecuteNonQuery();
            }

            return Ok(new { message = "Employee added successfully" });
        }

        // Delete an employee by ID
        [HttpDelete("{id}")]
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
    }
}
