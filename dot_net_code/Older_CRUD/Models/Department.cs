using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Older_CRUD.Models
{
    public class Department
    {
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }



        public static List<Department> GetAllDepartments()
        {
            string conStr = ConfigurationManager.ConnectionStrings["Employee_management"].ConnectionString;
            List<Department> deptList = new List<Department>();

            using (SqlConnection con = new SqlConnection(conStr))
            {
                string query = "SELECT * FROM Department";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    con.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Department d = new Department
                            {
                                DepartmentId = Convert.ToInt32(reader["DeaprtmentId"]),
                                DepartmentName = reader["DepartmentName"].ToString(),
                                
                            };

                            deptList.Add(d);
                        }
                    }
                }
            }

            return deptList;
        }

        public static bool Create(Department dept)
        {
            string conStr = ConfigurationManager.ConnectionStrings["Employee_management"].ConnectionString;

            // FIXED: Target the Department table and only insert DepartmentName 
            // (Assuming DepartmentId is an auto-incrementing Identity column in your DB)
            string query = "INSERT INTO Department (DepartmentName) VALUES (@DepartmentName)";

            try
            {
                using (SqlConnection con = new SqlConnection(conStr))
                {
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        // Safely bind the parameter
                        cmd.Parameters.AddWithValue("@DepartmentName", dept.DepartmentName);

                        con.Open();

                        // Run the query. ExecuteNonQuery returns the number of rows affected.
                        int rowsAffected = cmd.ExecuteNonQuery();

                        // Return true if the row was successfully added
                        return rowsAffected > 0;
                    }
                }
            }
            catch (SqlException ex)
            {
      
                if (ex.Number == 2627 || ex.Number == 2601)
                {
                    throw new Exception("This department name already exists.");
                }

                throw new Exception("A database error occurred while creating the department: " + ex.Message);
            }
        }
    }
}