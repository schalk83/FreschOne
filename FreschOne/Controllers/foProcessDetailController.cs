    using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using FreschOne.Models;
using System.Data;
using System.Text.Json;
using System;

namespace FreschOne.Controllers
{
    public class foProcessDetailController : BaseController
    {
        public foProcessDetailController(DatabaseHelper dbHelper, IConfiguration configuration)
            : base(dbHelper, configuration) { }

        private SqlConnection GetConnection() => new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

        public IActionResult Index(int userid, long? processId, long? stepId)
        {
            SetUserAccess(userid);
            ViewBag.userid = userid;
            ViewBag.ProcessDropdown = GetProcessSelectList();
            ViewBag.StepDropdown = processId.HasValue ? GetStepSelectList(processId.Value) : new List<SelectListItem>();

            ViewBag.SelectedProcessId = processId;
            ViewBag.SelectedStepId = stepId;

            var list = new List<foProcessDetail>();

            if (stepId == null)
                return View(list);

            ViewBag.StepDetailList = GetDetailsForStep(stepId.Value);
            return View(ViewBag.StepDetailList);
        }

        public IActionResult Create(int userid, long processId, long stepId)
        {
            SetUserAccess(userid);
            ViewBag.userid = userid;
            ViewBag.ProcessId = processId;
            ViewBag.StepId = stepId;
            ViewBag.FormTypes = GetFormTypeSelectList();

            var stepDetails = GetDetailsForStep(stepId);
            ViewBag.StepDetailList = stepDetails;
            bool isFirstDetail = !stepDetails.Any();
            ViewBag.IsFirstDetail = isFirstDetail;

            return View(new foProcessDetail
            {
                StepID = stepId,
                Parent = isFirstDetail,
                Active = true
            });
        }

        [HttpPost]
        public IActionResult Create(foProcessDetail detail, int userid, long processId, string action)
        {
            SetUserAccess(userid);
            ViewBag.userid = userid;
            ViewBag.FormTypes = GetFormTypeSelectList();

            var stepDetails = GetDetailsForStep((long)detail.StepID);
            ViewBag.StepDetailList = stepDetails;
            bool isFirstDetail = !stepDetails.Any();
            ViewBag.IsFirstDetail = isFirstDetail;
            detail.Parent = isFirstDetail;

            if (!IsQueryValid(detail.TableName, detail.ColumnQuery))
            {
                ModelState.AddModelError("ColumnQuery", "❌ Invalid SQL query. Please check your table name or column list.");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.StepId = detail.StepID;
                ViewBag.ProcessId = processId;
                return View(detail);
            }

            using (var conn = GetConnection())
            {
                var cmd = new SqlCommand(@"INSERT INTO foProcessDetail
                (StepID, TableName, ColumnQuery, FormType, ColumnCount, Parent, FKColumn, TableDescription, Active)
                VALUES
                (@StepID, @TableName, @ColumnQuery, @FormType, @ColumnCount, @Parent, @FKColumn, @TableDescription, @Active)", conn);

                cmd.Parameters.AddWithValue("@StepID", detail.StepID);
                cmd.Parameters.AddWithValue("@TableName", detail.TableName ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@ColumnQuery", detail.ColumnQuery ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@FormType", detail.FormType ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@ColumnCount", detail.ColumnCount);
                cmd.Parameters.AddWithValue("@Parent", detail.Parent);
                cmd.Parameters.AddWithValue("@FKColumn", detail.FKColumn ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@TableDescription", detail.TableDescription ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Active", detail.Active);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
            if (action == "addanother")
            {
                return RedirectToAction("Create", new { userid, processId, stepId = detail.StepID });
            }

            return RedirectToAction("Index", new { userid, processId, stepId = detail.StepID });
        }

        public IActionResult Edit(long id, int userid)
        {
            SetUserAccess(userid);
            ViewBag.userid = userid;
            ViewBag.FormTypes = GetFormTypeSelectList();

            foProcessDetail detail = null;
            long processId = 0;

            using (var conn = GetConnection())
            {
                var cmd = new SqlCommand("SELECT d.*, s.ProcessID FROM foProcessDetail d JOIN foProcessSteps s ON d.StepID = s.ID WHERE d.ID = @ID", conn);
                cmd.Parameters.AddWithValue("@ID", id);
                conn.Open();
                using var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    detail = new foProcessDetail
                    {
                        ID = (long)reader["ID"],
                        StepID = (long)reader["StepID"],
                        TableName = reader["TableName"]?.ToString(),
                        ColumnQuery = reader["ColumnQuery"]?.ToString(),
                        FormType = reader["FormType"]?.ToString(),
                        ColumnCount = reader["ColumnCount"] != DBNull.Value ? (int)reader["ColumnCount"] : 0,
                        Parent = reader["Parent"] != DBNull.Value && (bool)reader["Parent"],
                        FKColumn = reader["FKColumn"]?.ToString(),
                        TableDescription = reader["TableDescription"]?.ToString(),
                        Active = reader["Active"] != DBNull.Value && (bool)reader["Active"]
                    };
                    processId = (long)reader["ProcessID"];
                }
            }

            ViewBag.StepId = detail.StepID;
            ViewBag.ProcessId = processId;
            ViewBag.StepDetailList = GetDetailsForStep((long)detail.StepID);
            return View(detail);
        }

        [HttpPost]
        public IActionResult Edit(foProcessDetail detail, int userid, long processId, string action)
        {
            SetUserAccess(userid);
            ViewBag.userid = userid;
            ViewBag.FormTypes = GetFormTypeSelectList();
            ViewBag.StepDetailList = GetDetailsForStep((long)detail.StepID);

            if (!IsQueryValid(detail.TableName, detail.ColumnQuery))
            {
                ModelState.AddModelError("ColumnQuery", "❌ Invalid SQL query. Please check your table name or column list.");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.StepId = detail.StepID;
                ViewBag.ProcessId = processId;
                return View(detail);
            }

            using (var conn = GetConnection())
            {
                var cmd = new SqlCommand(@"UPDATE foProcessDetail SET
                    TableName = @TableName,
                    ColumnQuery = @ColumnQuery,
                    FormType = @FormType,
                    ColumnCount = @ColumnCount,
                    Parent = @Parent,
                    FKColumn = @FKColumn,
                    TableDescription = @TableDescription,
                    Active = @Active
                    WHERE ID = @ID", conn);

                cmd.Parameters.AddWithValue("@TableName", detail.TableName ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@ColumnQuery", detail.ColumnQuery ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@FormType", detail.FormType ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@ColumnCount", detail.ColumnCount);
                cmd.Parameters.AddWithValue("@Parent", detail.Parent);
                cmd.Parameters.AddWithValue("@FKColumn", detail.FKColumn ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@TableDescription", detail.TableDescription ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Active", detail.Active);
                cmd.Parameters.AddWithValue("@ID", detail.ID);

                conn.Open();
                cmd.ExecuteNonQuery();
            }

            if (action == "addanother")
            {
                return RedirectToAction("Create", new { userid, processId, stepId = detail.StepID });
            }

            return RedirectToAction("Index", new { userid, processId, stepId = detail.StepID });
        }

        [HttpPost]
        public IActionResult Delete(long id, int userid)
        {
            SetUserAccess(userid);
            long stepId;
            long processId;

            using (var conn = GetConnection())
            {
                var getCmd = new SqlCommand("SELECT d.StepID, s.ProcessID FROM foProcessDetail d JOIN foProcessSteps s ON d.StepID = s.ID WHERE d.ID = @ID", conn);
                getCmd.Parameters.AddWithValue("@ID", id);
                conn.Open();
                using var reader = getCmd.ExecuteReader();
                reader.Read();
                stepId = (long)reader["StepID"];
                processId = (long)reader["ProcessID"];

                var delCmd = new SqlCommand("DELETE FROM foProcessDetail WHERE ID = @ID", conn);
                delCmd.Parameters.AddWithValue("@ID", id);
                delCmd.ExecuteNonQuery();
            }

            return RedirectToAction("Index", new { userid, processId, stepId });
        }

        private List<foProcessDetail> GetDetailsForStep(long stepId)
        {
            var list = new List<foProcessDetail>();
            using var conn = GetConnection();
            var cmd = new SqlCommand("SELECT * FROM foProcessDetail WHERE StepID = @StepID ORDER BY ID", conn);
            cmd.Parameters.AddWithValue("@StepID", stepId);
            conn.Open();
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new foProcessDetail
                {
                    ID = (long)reader["ID"],
                    StepID = (long)reader["StepID"],
                    TableName = reader["TableName"]?.ToString(),
                    ColumnQuery = reader["ColumnQuery"]?.ToString(),
                    FormType = reader["FormType"]?.ToString(),
                    ColumnCount = reader["ColumnCount"] != DBNull.Value ? (int)reader["ColumnCount"] : 0,
                    Parent = reader["Parent"] != DBNull.Value && (bool)reader["Parent"],
                    FKColumn = reader["FKColumn"]?.ToString(),
                    TableDescription = reader["TableDescription"]?.ToString(),
                    Active = reader["Active"] != DBNull.Value && (bool)reader["Active"]
                });
            }
            return list;
        }

        private List<SelectListItem> GetFormTypeSelectList()
        {
            return new List<SelectListItem>
            {
                new SelectListItem { Value = "F", Text = "F" },
                new SelectListItem { Value = "T", Text = "T" }
            };
        }

        private List<SelectListItem> GetProcessSelectList()
        {
            var list = new List<SelectListItem>();
            using var conn = GetConnection();
            var cmd = new SqlCommand("SELECT ID, ProcessName FROM foProcess WHERE Active = 1", conn);
            conn.Open();
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new SelectListItem
                {
                    Value = reader["ID"].ToString(),
                    Text = reader["ProcessName"].ToString()
                });
            }
            return list;
        }

        private List<SelectListItem> GetStepSelectList(long processId)
        {
            var list = new List<SelectListItem>();
            using var conn = GetConnection();
            var cmd = new SqlCommand("SELECT ID, StepNo, StepDescription FROM foProcessSteps WHERE Active = 1 AND ProcessID = @ProcessID ORDER BY StepNo", conn);
            cmd.Parameters.AddWithValue("@ProcessID", processId);
            conn.Open();
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var stepNo = Convert.ToDecimal(reader["StepNo"]);
                var description = reader["StepDescription"]?.ToString() ?? "";

                list.Add(new SelectListItem
                {
                    Value = reader["ID"].ToString(),
                    Text = $"Step {stepNo:0.##} - {description}" // Emphasized label
                });
            }
            return list;
        }



        private bool IsQueryValid(string tableName, string columnQuery)
        {
            using var conn = GetConnection();
            var query = $"SELECT TOP 1 {columnQuery} FROM {tableName}";
            try
            {
                using var cmd = new SqlCommand(query, conn);
                conn.Open();
                using var reader = cmd.ExecuteReader();
                return true;
            }
            catch
            {
                return false;
            }
        }

        [HttpPost]
        public JsonResult TestQuery([FromBody] JsonElement data)
        {
            try
            {
                string tableName = data.GetProperty("tableName").GetString();
                string columnQuery = data.GetProperty("columnQuery").GetString();

                using var conn = GetConnection();
                var cmd = new SqlCommand($"SELECT TOP 1 {columnQuery} FROM {tableName}", conn);
                conn.Open();
                using var reader = cmd.ExecuteReader();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

     


    }
}
