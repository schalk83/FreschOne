using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Mvc.Rendering;
using FreschOne.Models;
using System.Collections.Generic;
using System.Data;

namespace FreschOne.Controllers
{
    public class foTableColumnsController : BaseController
    {
        public foTableColumnsController(DatabaseHelper dbHelper, IConfiguration configuration) : base(dbHelper, configuration) { }

        private SqlConnection GetConnection() =>
            new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

        public IActionResult Index(int userid, long? tableId = null)
        {
            SetUserAccess(userid);
            ViewBag.userid = userid;

            var tableList = new List<SelectListItem>();
            using (var conn = GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT ID, SchemaName, TableName FROM foTable WHERE Active = 1 ORDER BY SchemaName, TableName", conn);
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var id = (long)reader["ID"];
                    var label = $"{reader["SchemaName"]}.{reader["TableName"]}";
                    tableList.Add(new SelectListItem
                    {
                        Value = id.ToString(),
                        Text = label,
                        Selected = (tableId.HasValue && tableId.Value == id)
                    });
                }
            }

            ViewBag.TableList = tableList;
            ViewBag.SelectedTableId = tableId;

            var columns = new List<foTableColumns>();
            if (tableId.HasValue)
            {
                using var conn = GetConnection();
                conn.Open();
                var cmd = new SqlCommand("SELECT * FROM foTableColumns WHERE TableID = @TableID ORDER BY ColumnOrder", conn);
                cmd.Parameters.AddWithValue("@TableID", tableId.Value);
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    columns.Add(new foTableColumns
                    {
                        ID = (long)reader["ID"],
                        TableID = (long)reader["TableID"],
                        ColumnName = reader["ColumnName"].ToString(),
                        ColumnActualName = reader["ColumnActualName"].ToString(),
                        ColumnOrder = Convert.ToInt64(reader["ColumnOrder"]),
                        ColumnMaxLength = Convert.ToInt64(reader["ColumnMaxLength"]),
                        ColumnPrecision = Convert.ToInt64(reader["ColumnPrecision"]),
                        ColumnIsNullable = reader["ColumnIsNullable"] != DBNull.Value && Convert.ToBoolean(reader["ColumnIsNullable"]),
                        ColumnDataType = reader["ColumnDataType"]?.ToString(),
                        IsPrimaryKey = reader["IsPrimaryKey"] != DBNull.Value && Convert.ToBoolean(reader["IsPrimaryKey"]),
                        IsForeignKey = reader["IsForeignKey"] != DBNull.Value && Convert.ToBoolean(reader["IsForeignKey"]),
                        ForeignKeyTableName = reader["ForeignKeyTableName"]?.ToString(),
                    });
                }
            }

            columns = columns.Where(c => !c.IsSystemColumn).ToList();
            return View(columns);
        }

        public IActionResult Create(int userid, long tableId)
        {
            SetUserAccess(userid);
            ViewBag.userid = userid;
            ViewBag.TableID = tableId;

            ViewBag.ForeignKeyTableDropdown = GetForeignKeyTableDropdown();


            return View(new foTableColumns { TableID = tableId });
        }


        [HttpPost]
        public IActionResult Create(foTableColumns column, int userid)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.userid = userid;
                ViewBag.TableID = column.TableID;
                ViewBag.ForeignKeyTableDropdown = GetForeignKeyTableDropdown();
                return View(column);
            }

            using (var conn = GetConnection())
            {
                conn.Open();
                using var tx = conn.BeginTransaction();

                // 🔍 Check for duplicate ColumnName (case-insensitive)
                var checkCmd = new SqlCommand(@"
            SELECT COUNT(*) FROM foTableColumns
            WHERE TableID = @TableID AND LOWER(ColumnName) = LOWER(@ColumnName)", conn, tx);
                checkCmd.Parameters.AddWithValue("@TableID", column.TableID);
                checkCmd.Parameters.AddWithValue("@ColumnName", column.ColumnName);

                int existing = (int)checkCmd.ExecuteScalar();
                if (existing > 0)
                {
                    ModelState.AddModelError("ColumnName", "A column with this name already exists in the table.");
                    ViewBag.userid = userid;
                    ViewBag.TableID = column.TableID;
                    ViewBag.ForeignKeyTableDropdown = GetForeignKeyTableDropdown();
                    return View(column);
                }

                // 🧠 Determine ColumnActualName based on FK or attachment logic
                string actualName = column.ColumnName;

                if ((column.ColumnDataType?.ToLower() ?? "") == "attachment")
                {
                    actualName = $"attachment_{column.ColumnName}";
                }
                else if (column.IsForeignKey && !string.IsNullOrWhiteSpace(column.ForeignKeyTableName))
                {
                    actualName = $"{column.ColumnName}ID";
                }

                column.ColumnActualName = actualName;

                // 🛠 Inject system columns if this is the first for this table
                var colCountCmd = new SqlCommand("SELECT COUNT(*) FROM foTableColumns WHERE TableID = @TableID", conn, tx);
                colCountCmd.Parameters.AddWithValue("@TableID", column.TableID);
                int colCount = (int)colCountCmd.ExecuteScalar();

                if (colCount == 0)
                {
                    var ignoreCmd = new SqlCommand("SELECT ColumnName FROM foTableColumnsToIgnore ORDER BY ID", conn, tx);
                    using var reader = ignoreCmd.ExecuteReader();
                    var sysCols = new List<string>();
                    while (reader.Read())
                        sysCols.Add(reader.GetString(0));
                    reader.Close();

                    int sysOrder = 1;
                    foreach (var sysCol in sysCols)
                    {
                        var insertSysCmd = new SqlCommand(@"
                    INSERT INTO foTableColumns 
                    (TableID, ColumnName, ColumnActualName, ColumnOrder, ColumnMaxLength, ColumnPrecision, ColumnIsNullable, ColumnDataType, IsSystemColumn)
                    VALUES 
                    (@TableID, @ColumnName, @ColumnActualName, @ColumnOrder, 0, 0, 1, @ColumnDataType, 1)", conn, tx);

                        insertSysCmd.Parameters.AddWithValue("@TableID", column.TableID);
                        insertSysCmd.Parameters.AddWithValue("@ColumnName", sysCol);
                        insertSysCmd.Parameters.AddWithValue("@ColumnActualName", sysCol);
                        insertSysCmd.Parameters.AddWithValue("@ColumnOrder", sysOrder++);
                        insertSysCmd.Parameters.AddWithValue("@ColumnDataType", GetDefaultDataType(sysCol));

                        insertSysCmd.ExecuteNonQuery();
                    }

                    // Automatically set next order for user-defined column
                    column.ColumnOrder = sysCols.Count + 1;
                }

                // 🟢 Insert new user-defined column
                var insertCmd = new SqlCommand(@"
            INSERT INTO foTableColumns 
            (TableID, ColumnName, ColumnActualName, ColumnOrder, ColumnMaxLength, ColumnPrecision, ColumnIsNullable, ColumnDataType, 
             IsSystemColumn, IsPrimaryKey, IsForeignKey, ForeignKeyTableName)
            VALUES 
            (@TableID, @ColumnName, @ColumnActualName, @ColumnOrder, @ColumnMaxLength, @ColumnPrecision, @ColumnIsNullable, @ColumnDataType, 
             0, @IsPrimaryKey, @IsForeignKey, @ForeignKeyTableName)", conn, tx);

                insertCmd.Parameters.AddWithValue("@TableID", column.TableID);
                insertCmd.Parameters.AddWithValue("@ColumnName", column.ColumnName);
                insertCmd.Parameters.AddWithValue("@ColumnActualName", column.ColumnActualName);
                insertCmd.Parameters.AddWithValue("@ColumnOrder", column.ColumnOrder);
                insertCmd.Parameters.AddWithValue("@ColumnMaxLength", column.ColumnMaxLength);
                insertCmd.Parameters.AddWithValue("@ColumnPrecision", column.ColumnPrecision);
                insertCmd.Parameters.AddWithValue("@ColumnIsNullable", column.ColumnIsNullable);
                insertCmd.Parameters.AddWithValue("@ColumnDataType", column.ColumnDataType ?? "varchar");
                insertCmd.Parameters.AddWithValue("@IsPrimaryKey", column.IsPrimaryKey);
                insertCmd.Parameters.AddWithValue("@IsForeignKey", column.IsForeignKey);
                insertCmd.Parameters.AddWithValue("@ForeignKeyTableName", (object?)column.ForeignKeyTableName ?? DBNull.Value);

                insertCmd.ExecuteNonQuery();
                tx.Commit();
            }

            return RedirectToAction("Index", "foTableController", new { userid });
        }





        public IActionResult Edit(long id, int userid)
        {
            SetUserAccess(userid);
            ViewBag.userid = userid;

            foTableColumns column = null;
            using var conn = GetConnection();
            conn.Open();

            var cmd = new SqlCommand("SELECT * FROM foTableColumns WHERE ID = @ID", conn);
            cmd.Parameters.AddWithValue("@ID", id);
            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                column = new foTableColumns
                {
                    ID = (long)reader["ID"],
                    TableID = (long)reader["TableID"],
                    ColumnName = reader["ColumnName"].ToString(),
                    ColumnActualName = reader["ColumnActualName"].ToString(),
                    ColumnOrder = Convert.ToInt64(reader["ColumnOrder"]),
                    ColumnMaxLength = Convert.ToInt64(reader["ColumnMaxLength"]),
                    ColumnPrecision = Convert.ToInt64(reader["ColumnPrecision"]),
                    ColumnIsNullable = reader["ColumnIsNullable"] != DBNull.Value && Convert.ToBoolean(reader["ColumnIsNullable"]),
                    ColumnDataType = reader["ColumnDataType"]?.ToString(),
                    IsPrimaryKey = reader["IsPrimaryKey"] != DBNull.Value && Convert.ToBoolean(reader["IsPrimaryKey"]),
                    IsForeignKey = reader["IsForeignKey"] != DBNull.Value && Convert.ToBoolean(reader["IsForeignKey"]),
                    ForeignKeyTableName = reader["ForeignKeyTableName"]?.ToString(),
                };
            }

            ViewBag.ForeignKeyTableDropdown = GetForeignKeyTableDropdown();


            ViewBag.TableID = column.TableID;
            return View(column);
        }

        [HttpPost]
        public IActionResult Edit(foTableColumns column, int userid)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.userid = userid;
                ViewBag.TableID = column.TableID;
                ViewBag.ForeignKeyTableDropdown = GetForeignKeyTableDropdown();
                return View(column);
            }

            using (var conn = GetConnection())
            {
                conn.Open();

                // ❌ Check for duplicate column name (excluding current row)
                var checkCmd = new SqlCommand(@"
            SELECT COUNT(*) FROM foTableColumns
            WHERE TableID = @TableID AND LOWER(ColumnName) = LOWER(@ColumnName) AND ID != @ID", conn);

                checkCmd.Parameters.AddWithValue("@TableID", column.TableID);
                checkCmd.Parameters.AddWithValue("@ColumnName", column.ColumnName);
                checkCmd.Parameters.AddWithValue("@ID", column.ID);

                int count = (int)checkCmd.ExecuteScalar();
                if (count > 0)
                {
                    ModelState.AddModelError("ColumnName", "Another column with this name already exists in the table.");
                    ViewBag.userid = userid;
                    ViewBag.TableID = column.TableID;
                    ViewBag.ForeignKeyTableDropdown = GetForeignKeyTableDropdown();
                    return View(column);
                }

                // 🧠 Calculate ColumnActualName
                string actualName = column.ColumnName;

                if ((column.ColumnDataType?.ToLower() ?? "") == "attachment")
                {
                    actualName = $"attachment_{column.ColumnName}";
                }
                else if (column.IsForeignKey && !string.IsNullOrWhiteSpace(column.ForeignKeyTableName))
                {
                    actualName = $"{column.ColumnName}ID";
                }

                // ✅ Update the column
                var updateCmd = new SqlCommand(@"
            UPDATE foTableColumns SET 
                ColumnName = @ColumnName,
                ColumnActualName = @ColumnActualName,
                ColumnOrder = @ColumnOrder,
                ColumnMaxLength = @ColumnMaxLength,
                ColumnPrecision = @ColumnPrecision,
                ColumnIsNullable = @ColumnIsNullable,
                ColumnDataType = @ColumnDataType,
                IsPrimaryKey = @IsPrimaryKey,
                IsForeignKey = @IsForeignKey,
                ForeignKeyTableName = @ForeignKeyTableName
            WHERE ID = @ID", conn);

                updateCmd.Parameters.AddWithValue("@ColumnName", column.ColumnName);
                updateCmd.Parameters.AddWithValue("@ColumnActualName", actualName);
                updateCmd.Parameters.AddWithValue("@ColumnOrder", column.ColumnOrder);
                updateCmd.Parameters.AddWithValue("@ColumnMaxLength", column.ColumnMaxLength);
                updateCmd.Parameters.AddWithValue("@ColumnPrecision", column.ColumnPrecision);
                updateCmd.Parameters.AddWithValue("@ColumnIsNullable", column.ColumnIsNullable);
                updateCmd.Parameters.AddWithValue("@ColumnDataType", column.ColumnDataType ?? "varchar");
                updateCmd.Parameters.AddWithValue("@IsPrimaryKey", column.IsPrimaryKey);
                updateCmd.Parameters.AddWithValue("@IsForeignKey", column.IsForeignKey);
                updateCmd.Parameters.AddWithValue("@ForeignKeyTableName", (object?)column.ForeignKeyTableName ?? DBNull.Value);
                updateCmd.Parameters.AddWithValue("@ID", column.ID);

                updateCmd.ExecuteNonQuery();
            }

            return RedirectToAction("Index", "foTableController", new { userid });
        }



        [HttpPost]
        public IActionResult Delete(long id, int userid, long tableId)
        {
            using var conn = GetConnection();
            conn.Open();

            var cmd = new SqlCommand("DELETE FROM foTableColumns WHERE ID = @ID", conn);
            cmd.Parameters.AddWithValue("@ID", id);
            cmd.ExecuteNonQuery();

            return RedirectToAction("Index", "foTableController", new { userid });
        }

        private void AddSystemColumnsIfMissing(SqlConnection conn, SqlTransaction tx, long tableId)
        {
            var existingCountCmd = new SqlCommand("SELECT COUNT(*) FROM foTableColumns WHERE TableID = @TableID", conn, tx);
            existingCountCmd.Parameters.AddWithValue("@TableID", tableId);
            int count = (int)existingCountCmd.ExecuteScalar();

            if (count > 0) return; // already has columns (assume system columns added)

            var ignoreCmd = new SqlCommand("SELECT ColumnName FROM foTableColumnsToIgnore ORDER BY ID", conn, tx);
            using var reader = ignoreCmd.ExecuteReader();

            var systemColumns = new List<string>();
            while (reader.Read())
            {
                systemColumns.Add(reader["ColumnName"].ToString());
            }
            reader.Close();

            int order = 1;
            foreach (var col in systemColumns)
            {
                var insertCmd = new SqlCommand(@"
            INSERT INTO foTableColumns 
            (TableID, ColumnName, ColumnOrder, ColumnMaxLength, ColumnPrecision, ColumnIsNullable, ColumnDataType, IsSystemColumn)
            VALUES 
            (@TableID, @ColumnName, @ColumnOrder, 0, 0, 1, @ColumnDataType, 1)", conn, tx);

                insertCmd.Parameters.AddWithValue("@TableID", tableId);
                insertCmd.Parameters.AddWithValue("@ColumnName", col);
                insertCmd.Parameters.AddWithValue("@ColumnOrder", order++);
                insertCmd.Parameters.AddWithValue("@ColumnDataType", GetDefaultDataType(col));

                insertCmd.ExecuteNonQuery();
            }
        }

        private string GetDefaultDataType(string col)
        {
            if (col.EndsWith("Date")) return "datetime";
            if (col.EndsWith("ID")) return "bigint";
            if (col == "Active") return "bit";
            return "varchar";
        }

        private List<SelectListItem> GetForeignKeyTableDropdown()
        {
            var list = new List<SelectListItem>();
            using var conn = GetConnection();
            conn.Open();

            // Get allowed prefixes
            var prefixCmd = new SqlCommand("SELECT Prefix FROM foTablePrefixes WHERE Active = 1", conn);
            var prefixes = new List<string>();
            using (var reader = prefixCmd.ExecuteReader())
            {
                while (reader.Read())
                    prefixes.Add(reader.GetString(0));
            }

            if (prefixes.Count == 0) return list;

            // Build dynamic WHERE clause for prefix match
            string prefixFilter = string.Join(" OR ", prefixes.Select((p, i) => $"name LIKE @p{i}"));
            var tableCmd = new SqlCommand($"SELECT name FROM sys.tables WHERE {prefixFilter} ORDER BY name", conn);

            for (int i = 0; i < prefixes.Count; i++)
                tableCmd.Parameters.AddWithValue($"@p{i}", $"{prefixes[i]}%");

            using (var reader = tableCmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    var name = reader.GetString(0);
                    list.Add(new SelectListItem { Text = name, Value = name });
                }
            }

            return list;
        }

        [HttpPost]
        public IActionResult BulkEdit(List<foTableColumns> columns, int userid, long tableId)
        {
            if (columns == null || columns.Count == 0)
                return RedirectToAction("Index", "foTable", new { userid });

            using (var conn = GetConnection())
            {
                conn.Open();
                using var tx = conn.BeginTransaction();

                for (int i = 0; i < columns.Count; i++)
                {
                    var col = columns[i];
                    if (col.Deleted)
                    {
                        if (col.ID > 0)
                        {
                            var deleteCmd = new SqlCommand("DELETE FROM foTableColumns WHERE ID = @ID", conn, tx);
                            deleteCmd.Parameters.AddWithValue("@ID", col.ID);
                            deleteCmd.ExecuteNonQuery();
                        }
                        continue;
                    }

                    if (string.IsNullOrWhiteSpace(col.ColumnName))
                    {
                        ModelState.AddModelError($"columns[{i}].ColumnName", "Column name is required.");
                        continue;
                    }

                    if (string.IsNullOrWhiteSpace(col.ColumnDataType))
                    {
                        ModelState.AddModelError($"columns[{i}].ColumnDataType", "Data type is required.");
                        continue;
                    }

                    if (col.IsForeignKey && string.IsNullOrWhiteSpace(col.ForeignKeyTableName))
                    {
                        ModelState.AddModelError($"columns[{i}].ForeignKeyTableName", "FK table required.");
                        continue;
                    }

                    // 🔄 Calculate ColumnActualName
                    col.ColumnActualName = col.ColumnDataType == "attachment"
                        ? $"attachment_{col.ColumnName}"
                        : (col.IsForeignKey ? $"{col.ColumnName}ID" : col.ColumnName);

                    SqlCommand cmd;
                    if (col.ID > 0)
                    {
                        cmd = new SqlCommand(@"
                    UPDATE foTableColumns SET 
                        ColumnName = @ColumnName,
                        ColumnActualName = @ColumnActualName,
                        ColumnOrder = @ColumnOrder,
                        ColumnMaxLength = @ColumnMaxLength,
                        ColumnPrecision = @ColumnPrecision,
                        ColumnIsNullable = @ColumnIsNullable,
                        ColumnDataType = @ColumnDataType,
                        IsPrimaryKey = @IsPrimaryKey,
                        IsForeignKey = @IsForeignKey,
                        ForeignKeyTableName = @ForeignKeyTableName,
                        Active = @Active
                    WHERE ID = @ID", conn, tx);
                        cmd.Parameters.AddWithValue("@ID", col.ID);
                    }
                    else
                    {
                        cmd = new SqlCommand(@"
                    INSERT INTO foTableColumns 
                    (TableID, ColumnName, ColumnActualName, ColumnOrder, ColumnMaxLength, ColumnPrecision, ColumnIsNullable, ColumnDataType, 
                     IsPrimaryKey, IsForeignKey, ForeignKeyTableName, Active)
                    VALUES 
                    (@TableID, @ColumnName, @ColumnActualName, @ColumnOrder, @ColumnMaxLength, @ColumnPrecision, @ColumnIsNullable, @ColumnDataType, 
                     @IsPrimaryKey, @IsForeignKey, @ForeignKeyTableName, @Active)", conn, tx);
                        cmd.Parameters.AddWithValue("@TableID", tableId);
                    }

                    cmd.Parameters.AddWithValue("@ColumnName", col.ColumnName);
                    cmd.Parameters.AddWithValue("@ColumnActualName", col.ColumnActualName);
                    cmd.Parameters.AddWithValue("@ColumnOrder", col.ColumnOrder);
                    cmd.Parameters.AddWithValue("@ColumnMaxLength", col.ColumnMaxLength);
                    cmd.Parameters.AddWithValue("@ColumnPrecision", col.ColumnPrecision);
                    cmd.Parameters.AddWithValue("@ColumnIsNullable", col.ColumnIsNullable);
                    cmd.Parameters.AddWithValue("@ColumnDataType", col.ColumnDataType ?? "varchar");
                    cmd.Parameters.AddWithValue("@IsPrimaryKey", col.IsPrimaryKey);
                    cmd.Parameters.AddWithValue("@IsForeignKey", col.IsForeignKey);
                    cmd.Parameters.AddWithValue("@ForeignKeyTableName", (object?)col.ForeignKeyTableName ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Active", col.Active);

                    cmd.ExecuteNonQuery();
                }

                if (!ModelState.IsValid)
                {
                    tx.Rollback();
                    ViewBag.userid = userid;
                    ViewBag.TableID = tableId;
                    ViewBag.SqlTypes = new List<string> { "int", "bigint", "nvarchar", "varchar", "datetime", "bit", "decimal", "attachment" };
                    ViewBag.ForeignKeyTableDropdown = GetForeignKeyTableDropdown();
                    return View(columns);
                }

                tx.Commit();
                TempData["Message"] = "✅ Columns updated successfully.";
            }

            return RedirectToAction("BulkEdit", new { userid, tableId });
        }


        // CONTROLLER UPDATE
        public IActionResult BulkEdit(int userid, long tableId)
        {
            SetUserAccess(userid);
            ViewBag.userid = userid;
            ViewBag.TableID = tableId;

            var ignoreNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            using (var conn = GetConnection())
            {
                conn.Open();
                var ignoreCmd = new SqlCommand("SELECT ColumnName FROM foTableColumnsToIgnore", conn);
                using var reader = ignoreCmd.ExecuteReader();
                while (reader.Read())
                    ignoreNames.Add(reader.GetString(0));
            }

            var columns = new List<foTableColumns>();
            using (var conn = GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT * FROM foTableColumns WHERE TableID = @TableID AND Active = 1 AND IsSystemColumn = 0 ORDER BY ColumnOrder", conn);
                cmd.Parameters.AddWithValue("@TableID", tableId);
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var colName = reader["ColumnName"].ToString();
                    if (ignoreNames.Contains(colName)) continue;

                    columns.Add(new foTableColumns
                    {
                        ID = (long)reader["ID"],
                        TableID = (long)reader["TableID"],
                        ColumnName = colName,
                        ColumnActualName = reader["ColumnActualName"].ToString(),
                        ColumnOrder = Convert.ToInt64(reader["ColumnOrder"]),
                        ColumnMaxLength = Convert.ToInt64(reader["ColumnMaxLength"]),
                        ColumnPrecision = Convert.ToInt64(reader["ColumnPrecision"]),
                        ColumnIsNullable = reader["ColumnIsNullable"] != DBNull.Value && Convert.ToBoolean(reader["ColumnIsNullable"]),
                        ColumnDataType = reader["ColumnDataType"]?.ToString(),
                        IsPrimaryKey = reader["IsPrimaryKey"] != DBNull.Value && Convert.ToBoolean(reader["IsPrimaryKey"]),
                        IsForeignKey = reader["IsForeignKey"] != DBNull.Value && Convert.ToBoolean(reader["IsForeignKey"]),
                        ForeignKeyTableName = reader["ForeignKeyTableName"]?.ToString(),
                        IsSystemColumn = false,
                        Active = reader["Active"] != DBNull.Value && Convert.ToBoolean(reader["Active"])
                    });
                }
            }

            // Auto-insert ID column if table is empty
            if (!columns.Any())
            {
                columns.Insert(0, new foTableColumns
                {
                    ColumnName = "ID",
                    ColumnActualName = "ID",
                    ColumnOrder = 1,
                    ColumnDataType = "bigint",
                    ColumnMaxLength = 0,
                    ColumnPrecision = 0,
                    ColumnIsNullable = false,
                    IsPrimaryKey = true,
                    IsForeignKey = false,
                    ForeignKeyTableName = null,
                    IsSystemColumn = true,
                    Active = true,
                    TableID = tableId
                });
            }

            ViewBag.SqlTypes = new List<string> { "int", "bigint", "nvarchar", "varchar", "datetime", "bit", "decimal", "attachment" };
            ViewBag.ForeignKeyTableDropdown = GetForeignKeyTableDropdown();

            return View(columns);
        }








    }
}
