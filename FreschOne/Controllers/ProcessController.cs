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

            var query = "SELECT DISTINCT ID AS ProcessID, ProcessName, ProcessDescription FROM foProcess " +
            "WHERE ID IN ( select ProcessID from foProcessSteps " +
            "WHERE ISNULL ( UserID, 0 ) = " + userId + " AND Active = 1) UNION SELECT DISTINCT ID AS ProcessID,ProcessName, ProcessDescription " +
                            "FROM foProcess WHERE ID IN ( SELECT ProcessID FROM foProcessSteps WHERE ISNULL ( GroupID, 0 ) IN " +
                            "( SELECT GroupID FROM foUserGroups WHERE UserID = " + userId + " ) AND Active = 1)";


            using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            processList.Add(new foProcess
                            {
                                ID = (long)reader["ProcessID"],
                                ProcessName = reader["ProcessName"].ToString(),
                                ProcessDescription = reader["ProcessDescription"].ToString(),
                            });
                        }
                    }
                }
            }
            return View(processList);
        }
    }
}
