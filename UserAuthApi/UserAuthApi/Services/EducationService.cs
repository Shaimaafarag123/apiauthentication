using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

public class EducationService
{
    private readonly string _connectionString;

    public EducationService(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }

    //  method to test prerequisites for a course and student
    public bool TestPrerequisites(string course, long studentId, out int result)
    {
        result = 1; 

        try
        {
            int prerequisiteCount = GetPrerequisiteCount(course);

            if (prerequisiteCount > 0)
            {
                
                List<int> prerequisiteGroups = GetDistinctPrerequisiteGroups(course);

                
                foreach (var group in prerequisiteGroups)
                {
                    if (!ValidatePrerequisiteGroup(course, studentId, group))
                    {
                        result = 0; 
                        break;
                    }
                }
            }

            return true; 
        }
        catch (Exception ex)
        {
            
            Console.WriteLine($"Error in TestPrerequisites: {ex.Message}");
            result = -1; 
            return false;
        }
    }

    //  method to get the count of prerequisites for a given course
    private int GetPrerequisiteCount(string course)
    {
        try
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var adapter = new SqlDataAdapter("SELECT COUNT(*) AS Count FROM Education.CoursesPrerequisites WHERE CourseID = @Course", connection))
            {
                adapter.SelectCommand.Parameters.AddWithValue("@Course", course);

                var dataSet = new DataSet();
                adapter.Fill(dataSet);

                if (dataSet.Tables[0].Rows.Count > 0)
                {
                    return Convert.ToInt32(dataSet.Tables[0].Rows[0]["Count"]);
                }

                return 0; 
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in GetPrerequisiteCount: {ex.Message}");
            throw; 
        }
    }

    // method to get distinct prerequisite groups for a given course
    private List<int> GetDistinctPrerequisiteGroups(string course)
    {
        var groups = new List<int>();

        try
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var adapter = new SqlDataAdapter("SELECT DISTINCT PrerequisitGroup FROM Education.CoursesPrerequisites WHERE CourseID = @Course AND TakeTogether = 0", connection))
            {
                adapter.SelectCommand.Parameters.AddWithValue("@Course", course);

                var dataSet = new DataSet();
                adapter.Fill(dataSet);

                foreach (DataRow row in dataSet.Tables[0].Rows)
                {
                    groups.Add(Convert.ToInt32(row["PrerequisitGroup"]));
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in GetDistinctPrerequisiteGroups: {ex.Message}");
            throw;
        }

        return groups;
    }

    // method to validate whether the student meets the requirements for a given prerequisite group
    private bool ValidatePrerequisiteGroup(string course, long studentId, int group)
    {
        try
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var adapter = new SqlDataAdapter(@"
                SELECT COUNT(*) AS Count 
                FROM Education.CoursesRegistration CR 
                INNER JOIN Education.CoursesPrerequisites CP 
                ON CR.Course = CP.PrerequisitID AND CR.GradeID BETWEEN 1 AND 10
                WHERE CR.Student = @StudentId AND CP.PrerequisitGroup = @Group AND CP.CourseID = @Course", connection))
            {
                adapter.SelectCommand.Parameters.AddWithValue("@Course", course);
                adapter.SelectCommand.Parameters.AddWithValue("@StudentId", studentId);
                adapter.SelectCommand.Parameters.AddWithValue("@Group", group);

                var dataSet = new DataSet();
                adapter.Fill(dataSet);

                if (dataSet.Tables[0].Rows.Count > 0)
                {
                    int count = Convert.ToInt32(dataSet.Tables[0].Rows[0]["Count"]);
                    return count > 0; 
                }

                return false; 
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in ValidatePrerequisiteGroup: {ex.Message}");
            throw; 
        }
    }
}
