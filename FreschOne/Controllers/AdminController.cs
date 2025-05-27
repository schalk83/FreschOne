using FreschOne.Controllers;
using FreschOne.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using static System.Runtime.InteropServices.Marshalling.IIUnknownCacheStrategy;


namespace FreschOne.Controllers
{
    public class AdminController : BaseController
    {
        public AdminController(DatabaseHelper dbHelper, IConfiguration configuration) : base(dbHelper, configuration) { }

        public IActionResult Index(int userid)
        {
            ViewBag.userid = userid;

            SetUserAccess(userid);

            var tableList = new List<AdminTableInfo>();
            var query = "SELECT TableName, TableGroup FROM dbo.foAdminTables WHERE Active = 1 ORDER BY 2";

            using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            tableList.Add(new AdminTableInfo
                            {
                                TableName = reader["TableName"].ToString(),
                                TableGroup = reader["TableGroup"].ToString()
                            });
                        }
                    }
                }
            }
            return View(tableList);
        }
    }

    

}
