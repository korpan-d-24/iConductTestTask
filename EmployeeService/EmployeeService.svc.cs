using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Threading.Tasks;
using OneOf;
using OneOf.Types;

namespace EmployeeService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    public class Service1 : IEmployeeService
    {
        private readonly string connectionString = ConfigurationManager.ConnectionStrings["Data Source=(local);Initial Catalog=Test;User ID=sa;Password=pass@word1; "].ConnectionString;


        public async Task<OneOf<Employee, NotFound>> GetEmployeeById(int id)
        {
            Employee employee = GetEmployeeTree(id);
            if (employee == null)
                return new NotFound();

            return employee;
        }

        public async Task<OneOf<Success, Error<string>>> EnableEmployee(int id, int enable)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("UPDATE Employee SET Enable = @Enable WHERE ID = @ID", connection))
                {
                    command.Parameters.AddWithValue("@Enable", enable);
                    command.Parameters.AddWithValue("@ID", id);

                    int rowsAffected = command.ExecuteNonQuery();
                    if (rowsAffected == 0)
                        return new Error<string>("Can`t update employee status");
                }
            }

            return new Success();
        }

        private Employee GetEmployeeTree(int id)
        {
            Employee rootEmployee = null;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                Dictionary<int, Employee> employeeMap = new Dictionary<int, Employee>();

                using (SqlCommand command = new SqlCommand("SELECT * FROM Employee WHERE ID = @ID", connection))
                {
                    command.Parameters.AddWithValue("@ID", id);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            rootEmployee = new Employee
                            {
                                ID = (int)reader["ID"],
                                Name = (string)reader["Name"],
                                ManagerID = reader["ManagerID"] == DBNull.Value ? null : (int?)reader["ManagerID"],
                                Enable = (bool)reader["Enable"]
                            };

                            employeeMap.Add(rootEmployee.ID, rootEmployee);
                        }
                    }
                }

                if (rootEmployee != null)
                {
                    foreach (Employee employee in employeeMap.Values)
                    {
                        if (employee.ManagerID.HasValue && employeeMap.ContainsKey(employee.ManagerID.Value))
                        {
                            Employee manager = employeeMap[employee.ManagerID.Value];
                            if (manager.Subordinates == null)
                                manager.Subordinates = new List<Employee>();
                            manager.Subordinates.Add(employee);
                        }
                    }
                }
            }

            return rootEmployee;
        }

    }
}