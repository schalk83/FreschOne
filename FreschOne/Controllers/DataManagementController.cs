using FreschOne.Controllers;
using FreschOne.Models;
using Microsoft.AspNetCore.Mvc;


namespace FreschOne.Controllers
{
    public class DataManagementController : BaseController
    {
        public DataManagementController(DatabaseHelper dbHelper, IConfiguration configuration) : base(dbHelper, configuration) { }

        private List<foUserTable> GetUserTables(int userid)
        {
            var query = "SELECT * FROM dbo.foUserTable WHERE UserID = @UserID";
            var userAccessList = _dbHelper.ExecuteQuery<foUserTable>(query, new { UserID = userid });
            return userAccessList;
        }

        public IActionResult Index(int userid)
        {
            SetUserAccess(userid);

            // Retrieve user's access data where Active = 1
            var userAccessList = GetUserTables(userid).Where(x => x.Active).ToList();

            // Get the table prefixes and their descriptions
            var tablePrefixes = GetTablePrefixes();

            // Group the user access list by prefix using the descriptions
            var groupedAccess = userAccessList
                .GroupBy(x =>
                {
                    // Find the matching prefix by checking if the TableName starts with any prefix
                    var matchingPrefix = tablePrefixes.FirstOrDefault(p => x.TableName.StartsWith(p.Prefix));
                    if (matchingPrefix != null)
                    {
                        // Return the description associated with the prefix
                        return matchingPrefix.Description;
                    }
                    return "Other"; // If no match found, group under "Unknown"
                })
                .ToDictionary(g => g.Key, g => g.ToList());

            // Pass both grouped data and the table prefixes to the view
            return View(new Tuple<Dictionary<string, List<foUserTable>>, List<foTablePrefix>>(groupedAccess, tablePrefixes));
        }


        private List<foTablePrefix> GetTablePrefixes()
        {
            var query = "SELECT * FROM dbo.foTablePrefixes WHERE Active = 1";
            var prefixes = _dbHelper.ExecuteQuery<foTablePrefix>(query);
            return prefixes;
        }

    }

}
