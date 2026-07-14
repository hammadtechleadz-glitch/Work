using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace Older_CRUD.Models
{
    public class Employee
    {
        public int EmployeeId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public decimal Salary { get; set; }
        public DateTime HireDate { get; set; }
        public bool IsActive { get; set; }
        public int DepartmentId { get; set; }
        public static List<Employee> GetAllEmployees()
        {
            string conStr = ConfigurationManager.ConnectionStrings["Employee_management"].ConnectionString;
            List<Employee> empList = new List<Employee>();

            using (SqlConnection con = new SqlConnection(conStr))
            {
                string query = "SELECT * FROM Employee";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    con.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Employee e = new Employee
                            {
                                EmployeeId = Convert.ToInt32(reader["EmployeeId"]),
                                FirstName = reader["FirstName"].ToString(),
                                LastName = reader["LastName"].ToString(),
                                Email = reader["Email"].ToString(),
                                Phone = reader["Phone"] == DBNull.Value ? null : reader["Phone"].ToString(),

                                Salary = Convert.ToDecimal(reader["Salary"]),
                                HireDate = Convert.ToDateTime(reader["HireDate"]),
                                IsActive = Convert.ToBoolean(reader["IsActive"]),
                                DepartmentId = Convert.ToInt32(reader["DepartmentId"])
                            };

                            empList.Add(e);
                        }
                    }
                }
            }

            return empList;
        }
        public static bool Create(Employee emp)
        {
            // when we want to give the data from C# obj to the SQL obj
            string conStr = ConfigurationManager.ConnectionStrings["Employee_management"].ConnectionString;

            // 1. Explicitly name the columns and use "@" placeholders for security
            string query = "INSERT INTO Employee (FirstName, LastName, Email, Phone, Salary, HireDate, IsActive, DepartmentId) " +
                           "VALUES (@FirstName, @LastName, @Email, @Phone, @Salary, @HireDate, @IsActive, @DepartmentId)";

            try
            {
                using (SqlConnection con = new SqlConnection(conStr))
                {
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        // 2. Safely bind your Employee data to the SQL parameters
                        cmd.Parameters.AddWithValue("@FirstName", emp.FirstName);
                        cmd.Parameters.AddWithValue("@LastName", emp.LastName);
                        cmd.Parameters.AddWithValue("@Email", emp.Email);

                        // Handle nullable Phone safely—if it's null, send DBNull to SQL
                        cmd.Parameters.AddWithValue("@Phone", emp.Phone ?? (object)DBNull.Value);

                        cmd.Parameters.AddWithValue("@Salary", emp.Salary);
                        cmd.Parameters.AddWithValue("@HireDate", emp.HireDate);
                        cmd.Parameters.AddWithValue("@IsActive", emp.IsActive);
                        cmd.Parameters.AddWithValue("@DepartmentId", emp.DepartmentId);

                        con.Open();

                        // 3. Run the query. ExecuteNonQuery returns the number of rows affected.
                        int rowsAffected = cmd.ExecuteNonQuery();

                        // Return true if at least one row was successfully added
                        return rowsAffected > 0;
                    }
                }
            }
            catch (SqlException ex)
            {
                // 547 is the official SQL Server Error Number for a Foreign Key violation
                if (ex.Number == 547)
                {
                    throw new Exception("The Department ID you entered does not exist. Please check it and try again.");
                }

                // Handle unique constraint violations (e.g., duplicate email) just in case!
                if (ex.Number == 2627 || ex.Number == 2601)
                {
                    throw new Exception("This email address is already registered to another employee.");
                }

                // Catches any other general database issue (connection loss, bad table names, etc.)
                throw new Exception("A database error occurred: " + ex.Message);
            }
        }
    }
}