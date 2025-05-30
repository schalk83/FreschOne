using FreschOne.Controllers;
using FreschOne.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace FreschOne.Controllers
{
    public class DataManagementController : BaseController
    {
        public DataManagementController(DatabaseHelper dbHelper, IConfiguration configuration) : base(dbHelper, configuration) { }

        private List<foUserTable> GetUserTables(int userid)
        {
            var query = "SELECT * FROM dbo.foUserTable WHERE UserID = @UserID";
            return _dbHelper.ExecuteQuery<foUserTable>(query, new { UserID = userid });
        }

        public IActionResult Index(int userid)
        {
            SetUserAccess(userid);

            // Get user access tables (Active = 1)
            var userAccessList = GetUserTables(userid).Where(x => x.Active).ToList();

            // Get prefixes
            var tablePrefixes = GetTablePrefixes();

            // Group user tables by prefix description
            var groupedAccess = userAccessList
                .GroupBy(x =>
                {
                    var prefix = tablePrefixes.FirstOrDefault(p => x.TableName.StartsWith(p.Prefix));
                    return prefix != null ? prefix.Description : "Other";
                })
                .ToDictionary(g => g.Key, g => g.ToList());

            // Build FK child-parent mappings (tbl_tran_* only)
            var childParentMappings = GetChildParentMappings();

            // Pass mappings and data to view
            ViewBag.ChildParentMappings = childParentMappings;

            return View(new Tuple<Dictionary<string, List<foUserTable>>, List<foTablePrefix>>(groupedAccess, tablePrefixes));
        }

        private Dictionary<string, string> GetChildParentMappings()
        {
            var childParentMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            using (var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                conn.Open();

                string query = @"
                    SELECT parent.name AS ParentTable, child.name AS ChildTable
                    FROM sys.foreign_keys fk
                    INNER JOIN sys.tables child ON fk.parent_object_id = child.object_id
                    INNER JOIN sys.tables parent ON fk.referenced_object_id = parent.object_id
                    WHERE parent.name LIKE 'tbl_tran_%'";

                using var cmd = new SqlCommand(query, conn);
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var parentTable = reader["ParentTable"].ToString();
                    var childTable = reader["ChildTable"].ToString();

                    // Check if child table name contains parent table name (partial match logic)
                    if (childTable.Contains(parentTable, StringComparison.OrdinalIgnoreCase))
                    {
                        childParentMap[childTable] = parentTable;
                    }
                }
            }

            return childParentMap;
        }

        private List<foTablePrefix> GetTablePrefixes()
        {
            var query = "SELECT * FROM dbo.foTablePrefixes WHERE Active = 1";
            return _dbHelper.ExecuteQuery<foTablePrefix>(query);
        }
    }
}
