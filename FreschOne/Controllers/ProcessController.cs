using DocumentFormat.OpenXml.Spreadsheet;
using FreschOne.Controllers;
using FreschOne.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using static System.Runtime.InteropServices.Marshalling.IIUnknownCacheStrategy;


namespace FreschOne.Controllers
{
    public class ProcessController : BaseController
    {
        public ProcessController(DatabaseHelper dbHelper, IConfiguration configuration) : base(dbHelper, configuration) { }
        private SqlConnection GetConnection() => new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

        public IActionResult NewProcessIndex(int userId)
        {
            ViewBag.userid = userId;
            SetUserAccess(userId);

            var processList = new List<foProcess>();

            const string query = @"
        SELECT DISTINCT p.ID AS ProcessID,
                        p.ProcessName,
                        p.ProcessDescription
        FROM   foProcess p
        WHERE  p.ID IN (
                SELECT ps.ProcessID
                FROM   foProcessSteps ps
                WHERE  ISNULL(ps.UserID, 0) = @UserId
                  AND  ps.Active = 1
                  AND  ps.ID IN (SELECT StepID FROM foProcessDetail WHERE Active = 1)
              )

        UNION

        SELECT DISTINCT p.ID AS ProcessID,
                        p.ProcessName,
                        p.ProcessDescription
        FROM   foProcess p
        WHERE  p.ID IN (
                SELECT ps.ProcessID
                FROM   foProcessSteps ps
                WHERE  ISNULL(ps.GroupID, 0) IN (
                        SELECT ug.GroupID
                        FROM   foUserGroups ug
                        WHERE  ug.UserID = @UserId
                    )
                  AND  ps.Active = 1
                  AND  ps.ID IN (SELECT StepID FROM foProcessDetail WHERE Active = 1)
              );";

            using (var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                using var cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@UserId", userId);

                conn.Open();
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    processList.Add(new foProcess
                    {
                        ID = Convert.ToInt32(reader["ProcessID"]),  // 🔁 safer than (long)
                        ProcessName = reader["ProcessName"].ToString(),
                        ProcessDescription = reader["ProcessDescription"].ToString(),
                    });
                }
            }

            return View(processList);
        }
    }
}
