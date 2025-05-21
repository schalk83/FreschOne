using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using FreschOne.Models;
using System.Collections.Generic;
using System.Data;

namespace FreschOne.Controllers
{
    public class foTableController : BaseController
    {
        public foTableController(DatabaseHelper dbHelper, IConfiguration configuration) : base(dbHelper, configuration) { }

        private SqlConnection GetConnection() => new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

        public IActionResult Index(int userid)
        {
            SetUserAccess(userid);
            ViewBag.userid = userid;

            var tables = new List<foTable>();

            using (var conn = GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT * FROM foTable WHERE Active = 1", conn);
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    tables.Add(new foTable
                    {
                        ID = (long)reader["ID"],
                        SchemaName = reader["SchemaName"].ToString(),
                        TableName = reader["TableName"].ToString()
                    });
                }
            }

            return View(tables);
        }

        public IActionResult Create(int userid)
        {
            SetUserAccess(userid);
            ViewBag.userid = userid;

            return View(new foTable());
        }

        [HttpPost]
        public IActionResult Create(foTable table, int userid)
        {
            if (!ModelState.IsValid)
                return View(table);

            using (var conn = GetConnection())
            {
                var cmd = new SqlCommand(@"
                    INSERT INTO foTable (SchemaName, TableName, Active)
                    VALUES (@SchemaName, @TableName, 1)", conn);

                cmd.Parameters.AddWithValue("@SchemaName", table.SchemaName);
                cmd.Parameters.AddWithValue("@TableName", table.TableName);

                conn.Open();
                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("Index", new { userid });
        }

        public IActionResult Edit(long id, int userid)
        {
            SetUserAccess(userid);
            ViewBag.userid = userid;

            foTable table = null;

            using (var conn = GetConnection())
            {
                var cmd = new SqlCommand("SELECT * FROM foTable WHERE ID = @ID", conn);
                cmd.Parameters.AddWithValue("@ID", id);
                conn.Open();

                using var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    table = new foTable
                    {
                        ID = (long)reader["ID"],
                        SchemaName = reader["SchemaName"].ToString(),
                        TableName = reader["TableName"].ToString()
                    };
                }
            }

            return View(table);
        }

        [HttpPost]
        public IActionResult Edit(foTable table, int userid)
        {
            if (!ModelState.IsValid)
                return View(table);

            using (var conn = GetConnection())
            {
                var cmd = new SqlCommand(@"
                    UPDATE foTable SET 
                        SchemaName = @SchemaName,
                        TableName = @TableName
                    WHERE ID = @ID", conn);

                cmd.Parameters.AddWithValue("@SchemaName", table.SchemaName);
                cmd.Parameters.AddWithValue("@TableName", table.TableName);
                cmd.Parameters.AddWithValue("@ID", table.ID);

                conn.Open();
                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("Index", new { userid });
        }

        [HttpPost]
        public IActionResult Delete(long id, int userid)
        {
            using (var conn = GetConnection())
            {
                conn.Open();

                // Check if table is referenced in TableColumns
                var cmd = new SqlCommand("SELECT COUNT(*) FROM TableColumns WHERE TableID = @TableID", conn);
                cmd.Parameters.AddWithValue("@TableID", id);

                int count = (int)cmd.ExecuteScalar();
                if (count > 0)
                {
                    TempData["ErrorMessage"] = $"❌ This table is linked to {count} column(s) and cannot be deleted. Remove columns first before trying again.";
                    return RedirectToAction("Index", new { userid });
                }

                // Soft delete
                var deleteCmd = new SqlCommand("UPDATE foTable SET Active = 0 WHERE ID = @ID", conn);
                deleteCmd.Parameters.AddWithValue("@ID", id);
                deleteCmd.ExecuteNonQuery();

                TempData["Message"] = "✅ Table deleted successfully.";
            }

            return RedirectToAction("Index", new { userid });
        }

        [HttpGet]
        public IActionResult Script(int tableId, int userid)
        {
            SetUserAccess(userid);
            ViewBag.userid = userid;
            ViewBag.tableId = tableId;

            var systemColumns = new List<(string Name, string Type)>();

            using (var conn = GetConnection())
            {
                var cmd = new SqlCommand("SELECT ColumnName FROM foTableColumnsToIgnore", conn);
                conn.Open();

                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var colName = reader["ColumnName"].ToString();
                    var colType = colName switch
                    {
                        "Active" => "bit",
                        _ when colName.Contains("UserID") => "bigint",
                        _ when colName.Contains("Date") => "datetime",
                        _ => "nvarchar(max)"
                    };
                    systemColumns.Add((colName, colType));
                }
            }

            var prefixes = new List<(string Prefix, string Description)>();

            using (var conn = GetConnection())
            {
                var prefixCmd = new SqlCommand("SELECT Prefix, Description FROM foTablePrefixes WHERE Active = 1", conn);
                conn.Open();

                using var reader = prefixCmd.ExecuteReader();
                while (reader.Read())
                {
                    prefixes.Add((reader["Prefix"].ToString(), reader["Description"].ToString()));
                }
            }

            var foreignKeyTables = new List<string>();

            using (var conn = GetConnection())
            {
                var cmd = new SqlCommand(@"
        SELECT t.name
        FROM sys.tables t
        JOIN foTablePrefixes p ON t.name LIKE p.Prefix + '%'
        ORDER BY t.name", conn);

                conn.Open();
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                    foreignKeyTables.Add(reader["name"].ToString());
            }

            string matchedPrefix = "";
            if (tableId > 0)
            {
                using var conn = GetConnection();
                var cmd = new SqlCommand("SELECT SchemaName, TableName,ColumnNames FROM foTable WHERE ID = @ID", conn);
                cmd.Parameters.AddWithValue("@ID", tableId);
                conn.Open();

                using var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    var schema = reader["SchemaName"]?.ToString();
                    var fullName = reader["TableName"]?.ToString();
                    var columnNames = reader["ColumnNames"]?.ToString();

                    ViewBag.SchemaName = schema;
                    ViewBag.TableName = fullName;
                    ViewBag.ColumnNames = columnNames; // 👈 Add this

                    matchedPrefix = prefixes
                        .FirstOrDefault(p => fullName != null && fullName.ToLowerInvariant().StartsWith(p.Prefix.ToLowerInvariant()))
                        .Prefix ?? "";

                    ViewBag.BaseTableName = string.IsNullOrEmpty(matchedPrefix)
                        ? fullName
                        : fullName?.Substring(matchedPrefix.Length);

                    ViewBag.MatchedPrefix = matchedPrefix;
                }

            }
            else
            {
                ViewBag.SchemaName = "dbo";
                ViewBag.TableName = "";
                ViewBag.BaseTableName = "";
                ViewBag.MatchedPrefix = "";
            }

            var savedColumns = new List<foTableColumns>();
            if (tableId > 0)
            {
                using var conn = GetConnection();
                var cmd = new SqlCommand("SELECT * FROM foTableColumns WHERE TableID = @TableID AND Active = 1", conn);
                cmd.Parameters.AddWithValue("@TableID", tableId);
                conn.Open();

                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    savedColumns.Add(new foTableColumns
                    {
                        ColumnName = reader["ColumnName"].ToString(),
                        ColumnDataType = reader["ColumnDataType"].ToString(),
                        ColumnLength_Precision = reader["ColumnLength_Precision"].ToString(),
                        IsNullable = (bool)reader["IsNullable"],
                        ForeignKeyTable = reader["ForeignKeyTable"]?.ToString(),
                        Attachment = (bool)reader["Attachment"],
                        Geo = (bool)reader["Geo"],

                    });
                }
            }

            ViewBag.SavedColumns = savedColumns;

            ViewBag.ForeignKeyTables = foreignKeyTables;

            ViewBag.Prefixes = prefixes;

            ViewBag.SystemColumns = systemColumns;
            return View();
        }

        [HttpPost]
        public IActionResult SaveGeneratedTable([FromBody] foTable table, int userid)
        {
            if (string.IsNullOrWhiteSpace(table.SchemaName) || string.IsNullOrWhiteSpace(table.TableName))
                return BadRequest("Missing schema or table name.");

            long tableId;

            using (var conn = GetConnection())
            {
                conn.Open();

                // 🔍 Check if table already exists
                var checkCmd = new SqlCommand(@"
            SELECT ID FROM foTable 
            WHERE SchemaName = @SchemaName AND TableName = @TableName", conn);

                checkCmd.Parameters.AddWithValue("@SchemaName", table.SchemaName);
                checkCmd.Parameters.AddWithValue("@TableName", table.TableName);

                var existingId = checkCmd.ExecuteScalar();

                if (existingId != null)
                {
                    // 🔁 Update existing
                    tableId = (long)existingId;

                    var updateCmd = new SqlCommand(@"
                UPDATE foTable 
                SET ColumnNames = @ColumnNames, Script = @Script
                WHERE ID = @ID", conn);

                    updateCmd.Parameters.AddWithValue("@ColumnNames", (object?)table.ColumnNames ?? DBNull.Value);
                    updateCmd.Parameters.AddWithValue("@Script", (object?)table.Script ?? DBNull.Value);
                    updateCmd.Parameters.AddWithValue("@ID", tableId);

                    updateCmd.ExecuteNonQuery();
                }
                else
                {
                    // 🆕 Insert new
                    var insertCmd = new SqlCommand(@"
                INSERT INTO foTable (SchemaName, TableName, ColumnNames, Script, Active)
                OUTPUT INSERTED.ID
                VALUES (@SchemaName, @TableName, @ColumnNames, @Script, 1)", conn);

                    insertCmd.Parameters.AddWithValue("@SchemaName", table.SchemaName);
                    insertCmd.Parameters.AddWithValue("@TableName", table.TableName);
                    insertCmd.Parameters.AddWithValue("@ColumnNames", (object?)table.ColumnNames ?? DBNull.Value);
                    insertCmd.Parameters.AddWithValue("@Script", (object?)table.Script ?? DBNull.Value);

                    tableId = (long)insertCmd.ExecuteScalar();
                }
            }

            return Json(new { tableId });
        }


        [HttpPost]
        public IActionResult SaveTableColumns([FromBody] SaveTableColumnsRequest request)
        {
            if (request.Columns == null || request.Columns.Count == 0)
                return BadRequest("No columns provided.");

            using (var conn = GetConnection())
            {
                conn.Open();

                var deleteCmd = new SqlCommand("DELETE FROM foTableColumns WHERE TableID = @TableID", conn);
                deleteCmd.Parameters.AddWithValue("@TableID", request.TableID);
                deleteCmd.ExecuteNonQuery();

                foreach (var col in request.Columns)
                {
                    var cmd = new SqlCommand(@"
                INSERT INTO foTableColumns 
                    (TableID, ColumnName, ColumnDataType, ColumnLength_Precision, IsNullable, ForeignKeyTable, Attachment, Geo, Active)
                VALUES 
                    (@TableID, @ColumnName, @ColumnDataType, @ColumnLength_Precision, @IsNullable, @ForeignKeyTable, @Attachment, @Geo, 1)", conn);

                    cmd.Parameters.AddWithValue("@TableID", request.TableID);
                    cmd.Parameters.AddWithValue("@ColumnName", col.ColumnName);
                    cmd.Parameters.AddWithValue("@ColumnDataType", col.ColumnDataType);
                    cmd.Parameters.AddWithValue("@ColumnLength_Precision", (object?)col.ColumnLength_Precision ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@IsNullable", col.IsNullable);
                    cmd.Parameters.AddWithValue("@ForeignKeyTable", (object?)col.ForeignKeyTable ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Attachment", col.Attachment);
                    cmd.Parameters.AddWithValue("@Geo", col.Geo);

                    cmd.ExecuteNonQuery();
                }
            }

            return Ok();
        }


        public class SaveTableColumnsRequest
        {
            public long TableID { get; set; }
            public List<foTableColumns> Columns { get; set; }
        }

    }

}