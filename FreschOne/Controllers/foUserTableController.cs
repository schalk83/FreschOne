using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Mvc.Rendering;
using FreschOne.Models;

namespace FreschOne.Controllers
{
    public class foUserTableController : BaseController
    {
        public foUserTableController(DatabaseHelper dbHelper, IConfiguration configuration) : base(dbHelper, configuration) { }

        private SqlConnection GetConnection() =>
            new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

        public IActionResult Index(int userid, long? l_user)
        {
            SetUserAccess(userid);
            ViewBag.userid = userid;
            ViewBag.l_user = l_user;
            ViewBag.Users = GetUserSelectList(l_user ?? 0);

            var list = new List<foUserTable>();

            if (l_user.HasValue)
            {
                using (var conn = GetConnection())
                {
                    var cmd = new SqlCommand(@"
                        SELECT ID, UserID, TableName, ReadWriteAccess, Active 
                        FROM foUserTable 
                        WHERE Active = 1 AND UserID = @UserID", conn);

                    cmd.Parameters.AddWithValue("@UserID", l_user);
                    conn.Open();

                    using var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        list.Add(new foUserTable
                        {
                            ID = (long)reader["ID"],
                            UserID = (long)reader["UserID"],
                            TableName = reader["TableName"].ToString(),
                            ReadWriteAccess = reader["ReadWriteAccess"].ToString(),
                            Active = (bool)reader["Active"]
                        });
                    }
                }
            }

            return View(list);
        }

        public IActionResult Create(int userid, long l_user)
        {
            SetUserAccess(userid);
            ViewBag.userid = userid;
            ViewBag.AccessOptions = GetAccessSelectList();
            ViewBag.TableOptions = GetAvailableTableList(l_user);

            return View(new foUserTable { UserID = l_user });
        }

        [HttpPost]
        public IActionResult Create(foUserTable model, int userid)
        {
            using (var conn = GetConnection())
            {
                var check = new SqlCommand(@"
                    SELECT COUNT(*) FROM foUserTable 
                    WHERE UserID = @UserID AND TableName = @TableName AND Active = 1", conn);

                check.Parameters.AddWithValue("@UserID", model.UserID);
                check.Parameters.AddWithValue("@TableName", model.TableName);
                conn.Open();

                int exists = (int)check.ExecuteScalar();
                if (exists > 0)
                {
                    ModelState.AddModelError("TableName", "This table is already assigned to the user.");
                }
            }

            if (!ModelState.IsValid)
            {
                ViewBag.userid = userid;
                ViewBag.AccessOptions = GetAccessSelectList();
                ViewBag.TableOptions = GetAvailableTableList(model.UserID);
                return View(model);
            }

            using (var conn = GetConnection())
            {
                var insert = new SqlCommand(@"
                    INSERT INTO foUserTable (UserID, TableName, ReadWriteAccess, Active)
                    VALUES (@UserID, @TableName, @Access, 1)", conn);

                insert.Parameters.AddWithValue("@UserID", model.UserID);
                insert.Parameters.AddWithValue("@TableName", model.TableName);
                insert.Parameters.AddWithValue("@Access", model.ReadWriteAccess);

                conn.Open();
                insert.ExecuteNonQuery();
            }

            return RedirectToAction("Index", new { userid, l_user = model.UserID });
        }

        public IActionResult Edit(int id, int userid)
        {
            SetUserAccess(userid);
            ViewBag.userid = userid;

            foUserTable model = null;

            using (var conn = GetConnection())
            {
                var cmd = new SqlCommand("SELECT * FROM foUserTable WHERE ID = @ID", conn);
                cmd.Parameters.AddWithValue("@ID", id);
                conn.Open();

                using var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    model = new foUserTable
                    {
                        ID = (long)reader["ID"],
                        UserID = (long)reader["UserID"],
                        TableName = reader["TableName"].ToString(),
                        ReadWriteAccess = reader["ReadWriteAccess"].ToString(),
                        Active = (bool)reader["Active"]
                    };
                }
            }

            ViewBag.AccessOptions = GetAccessSelectList();
            ViewBag.TableOptions = GetAvailableTableList(model.UserID, model.TableName); // include current table
            return View(model);
        }

        [HttpPost]
        public IActionResult Edit(foUserTable model, int userid)
        {
            using (var conn = GetConnection())
            {
                var check = new SqlCommand(@"
                    SELECT COUNT(*) FROM foUserTable 
                    WHERE UserID = @UserID AND TableName = @TableName AND ID <> @ID AND Active = 1", conn);

                check.Parameters.AddWithValue("@UserID", model.UserID);
                check.Parameters.AddWithValue("@TableName", model.TableName);
                check.Parameters.AddWithValue("@ID", model.ID);

                conn.Open();
                int exists = (int)check.ExecuteScalar();
                if (exists > 0)
                {
                    ModelState.AddModelError("TableName", "This table is already assigned to the user.");
                }
            }

            if (!ModelState.IsValid)
            {
                ViewBag.userid = userid;
                ViewBag.AccessOptions = GetAccessSelectList();
                ViewBag.TableOptions = GetAvailableTableList(model.UserID, model.TableName);
                return View(model);
            }

            using (var conn = GetConnection())
            {
                var update = new SqlCommand(@"
                    UPDATE foUserTable 
                    SET TableName = @TableName, ReadWriteAccess = @Access 
                    WHERE ID = @ID", conn);

                update.Parameters.AddWithValue("@ID", model.ID);
                update.Parameters.AddWithValue("@TableName", model.TableName);
                update.Parameters.AddWithValue("@Access", model.ReadWriteAccess);

                conn.Open();
                update.ExecuteNonQuery();
            }

            return RedirectToAction("Index", new { userid, l_user = model.UserID });
        }

        [HttpPost]
        public IActionResult Delete(int id, int userid, long l_user)
        {
            using (var conn = GetConnection())
            {
                var cmd = new SqlCommand("DELETE FROM foUserTable WHERE ID = @ID", conn);
                cmd.Parameters.AddWithValue("@ID", id);
                conn.Open();
                cmd.ExecuteNonQuery();
            }

            TempData["Message"] = "✅ Table access removed.";
            return RedirectToAction("Index", new { userid, l_user });
        }

        private List<SelectListItem> GetAccessSelectList()
        {
            return new List<SelectListItem>
            {
                new SelectListItem { Value = "R", Text = "Read Only" },
                new SelectListItem { Value = "RW", Text = "Read/Write" }
            };
        }

        private List<SelectListItem> GetAvailableTableList(long userId, string? currentTable = null)
        {
            var list = new List<SelectListItem>();
            var prefixes = new List<string>();
            var assigned = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            using (var conn = GetConnection())
            {
                conn.Open();

                // Step 1: get allowed prefixes
                var prefixCmd = new SqlCommand("SELECT Prefix FROM foTablePrefixes", conn);
                using (var reader = prefixCmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        prefixes.Add(reader["Prefix"].ToString());
                    }
                }

                // Step 2: get already assigned tables
                var assignedCmd = new SqlCommand("SELECT TableName FROM foUserTable WHERE UserID = @UserID AND Active = 1", conn);
                assignedCmd.Parameters.AddWithValue("@UserID", userId);
                using (var reader = assignedCmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        assigned.Add(reader["TableName"].ToString());
                    }
                }

                // Step 3: get all sys.tables and filter
                var sysTableCmd = new SqlCommand("SELECT name FROM sys.tables", conn);
                using (var reader = sysTableCmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var name = reader["name"].ToString();

                        if (prefixes.Any(p => name.StartsWith(p)) &&
                            (!assigned.Contains(name) || name == currentTable))
                        {
                            list.Add(new SelectListItem
                            {
                                Value = name,
                                Text = name
                            });
                        }
                    }
                }
            }

            return list.OrderBy(i => i.Text).ToList();
        }

        private List<SelectListItem> GetUserSelectList(long selectedId = 0)
        {
            var list = new List<SelectListItem>();

            using (var conn = GetConnection())
            {
                var cmd = new SqlCommand("SELECT ID, FirstName, LastName FROM foUsers WHERE Active = 1", conn);
                conn.Open();

                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var id = (long)reader["ID"];
                    list.Add(new SelectListItem
                    {
                        Value = id.ToString(),
                        Text = $"{reader["FirstName"]} {reader["LastName"]}",
                        Selected = id == selectedId
                    });
                }
            }

            return list.OrderBy(x => x.Text).ToList();
        }
    }
}
