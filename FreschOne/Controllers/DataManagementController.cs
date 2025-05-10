using FreschOne.Controllers;
using FreschOne.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Metadata.Internal;


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


            // Find child tables that reference a tbl_tran_ parent
            var fkChildToTranParent = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            string query = @"
SELECT DISTINCT child.name
FROM sys.foreign_keys fk
INNER JOIN sys.tables child ON fk.parent_object_id = child.object_id
INNER JOIN sys.tables parent ON fk.referenced_object_id = parent.object_id
WHERE parent.name LIKE 'tbl_tran_%'";

            using (var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                conn.Open();
                using var cmd = new SqlCommand(query, conn);
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    fkChildToTranParent.Add(reader.GetString(0));
                }
            }

            // Store FK table info in ViewBag
            ViewBag.ForeignKeyChildTables = fkChildToTranParent;


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
