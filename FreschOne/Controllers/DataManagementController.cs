using FreschOne.Controllers;
using FreschOne.Models;
using Microsoft.AspNetCore.Mvc;


namespace FreschOne.Controllers
{
    public class DataManagementController : BaseController
    {
        public DataManagementController(DatabaseHelper dbHelper, IConfiguration configuration) : base(dbHelper, configuration) { }

        public IActionResult Index(int userid)  
        {

            SetUserAccess(userid);

            // Retrieve user's access data where Active = 1
            var userAccessList = GetUserTables(userid).Where(x => x.Active).ToList();

            // Group the table names by prefix (tbl_tran_ and tbl_md_)
            var groupedAccess = userAccessList
                .GroupBy(x => x.TableName.StartsWith("tbl_tran_") ? "Transaction Tables" : "Maintenance Tables")
                .ToDictionary(g => g.Key, g => g.ToList());

            // Pass grouped data to the view
            return View(groupedAccess);
        }

        private List<foUserTable> GetUserTables(int userid)
        {
            var query = "SELECT * FROM dbo.foUserTable WHERE UserID = @UserID";
            var userAccessList = _dbHelper.ExecuteQuery<foUserTable>(query, new { UserID = userid });
            return userAccessList;
        }
    }

}
