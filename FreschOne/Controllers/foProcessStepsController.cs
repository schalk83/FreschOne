using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using FreschOne.Models;
using System;

namespace FreschOne.Controllers
{
    public class foProcessStepsController : BaseController
    {
        public foProcessStepsController(DatabaseHelper dbHelper, IConfiguration configuration) : base(dbHelper, configuration) { }

        private SqlConnection GetConnection() => new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

        public IActionResult Index(long? processid, int userid)
        {
            SetUserAccess(userid);
            ViewBag.userid = userid;
            ViewBag.ProcessID = processid;
            ViewBag.ProcessDropdown = GetProcessSelectList();

            if (processid == null)
                return View(new List<foProcessSteps>());

            var steps = GetStepsForProcess(processid.Value);
            var stepDetailsMap = GetStepDetailsForProcess(processid.Value);
            ViewBag.StepDetailsMap = stepDetailsMap;

            return View(steps);
        }

     

        public IActionResult Create(long processid, int userid)
        {
            SetUserAccess(userid);
            ViewBag.userid = userid;
            ViewBag.ProcessID = processid;
            ViewBag.ProcessDropdown = GetProcessSelectList();
            ViewBag.GroupDropdown = GetGroupSelectList();
            ViewBag.UserDropdown = GetUserSelectList();
            ViewBag.ProcessStepList = GetStepsForProcess(processid);

            decimal nextStepNo = 1;

            using (var conn = GetConnection())
            {
                var cmd = new SqlCommand("SELECT ISNULL(MAX(StepNo), 0) + 1 FROM foProcessSteps WHERE ProcessID = @ProcessID", conn);
                cmd.Parameters.AddWithValue("@ProcessID", processid);
                conn.Open();
                var result = cmd.ExecuteScalar();
                nextStepNo = Convert.ToDecimal(result);
            }

            var model = new foProcessSteps
            {
                ProcessID = processid,
                StepNo = nextStepNo,
                AssignUserID = null,
                AssignGroupID = null,
                Active = true
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult Create(foProcessSteps model, int userid, string action)
        {
            SetUserAccess(userid);
            ViewBag.userid = userid;
            ViewBag.ProcessID = model.ProcessID;

            bool isDuplicate = false;
            using (var conn = GetConnection())
            {
                var cmd = new SqlCommand(@"
            SELECT COUNT(*) 
            FROM foProcessSteps 
            WHERE ProcessID = @ProcessID AND StepNo = @StepNo", conn);

                cmd.Parameters.AddWithValue("@ProcessID", model.ProcessID);
                cmd.Parameters.AddWithValue("@StepNo", model.StepNo);
                conn.Open();
                isDuplicate = (int)cmd.ExecuteScalar() > 0;
            }

            if (isDuplicate)
            {
                ModelState.AddModelError("StepNo", $"Step No {model.StepNo} already exists in this process.");
                ViewBag.HighlightStepNo = model.StepNo;
            }

            if (!ModelState.IsValid)
            {
                ViewBag.ProcessDropdown = GetProcessSelectList();
                ViewBag.GroupDropdown = GetGroupSelectList();
                ViewBag.UserDropdown = GetUserSelectList();
                ViewBag.ProcessStepList = GetStepsForProcess(model.ProcessID);
                return View(model);
            }

            using (var conn = GetConnection())
            {
                var cmd = new SqlCommand(@"
            INSERT INTO foProcessSteps (ProcessID, StepNo, GroupID, UserID, StepDescription, Active)
            VALUES (@ProcessID, @StepNo, @GroupID, @UserID, @StepDescription, @Active)", conn);

                cmd.Parameters.AddWithValue("@ProcessID", model.ProcessID);
                cmd.Parameters.AddWithValue("@StepNo", model.StepNo);
                cmd.Parameters.AddWithValue("@GroupID", (object?)model.AssignGroupID ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@UserID", (object?)model.AssignUserID ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@StepDescription", (object?)model.StepDescription ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Active", model.Active);

                conn.Open();
                cmd.ExecuteNonQuery();
            }

            if (action == "addanother")
            {
                return RedirectToAction("Create", new { processid = model.ProcessID, userid });
            }

            return RedirectToAction("Index", new { processid = model.ProcessID, userid });
        }

        public IActionResult Edit(long id, int userid)
        {
            SetUserAccess(userid);
            ViewBag.userid = userid;

            foProcessSteps model = null;

            using (var conn = GetConnection())
            {
                var cmd = new SqlCommand(@"
            SELECT ps.*, 
                   g.Description AS GroupName,
                   u.FirstName + ' ' + u.LastName AS UserName
            FROM foProcessSteps ps
            LEFT JOIN foGroups g ON ps.GroupID = g.ID
            LEFT JOIN foUsers u ON ps.UserID = u.ID
            WHERE ps.ID = @ID", conn);

                cmd.Parameters.AddWithValue("@ID", id);
                conn.Open();

                using var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    model = new foProcessSteps
                    {
                        ID = (long)reader["ID"],
                        ProcessID = (long)reader["ProcessID"],
                        StepNo = (decimal)reader["StepNo"],
                        AssignGroupID = reader["GroupID"] as long?,
                        AssignUserID = reader["UserID"] as long?,
                        StepDescription = reader["StepDescription"]?.ToString(),
                        GroupName = reader["GroupName"]?.ToString(),
                        UserName = reader["UserName"]?.ToString(),
                        Active = reader["Active"] != DBNull.Value && (bool)reader["Active"]
                    };
                }
            }

            if (model == null)
                return NotFound();

            ViewBag.ProcessID = model.ProcessID;
            ViewBag.ProcessDropdown = GetProcessSelectList();
            ViewBag.GroupDropdown = GetGroupSelectList();
            ViewBag.UserDropdown = new SelectList(GetUserSelectList(), "Value", "Text", model.AssignUserID);
            ViewBag.ProcessStepList = GetStepsForProcess(model.ProcessID);

            return View(model);
        }

        [HttpPost]
        public IActionResult Edit(foProcessSteps model, int userid, string action)
        {
            SetUserAccess(userid);
            ViewBag.userid = userid;
            ViewBag.ProcessID = model.ProcessID;

            bool isDuplicate = false;
            using (var conn = GetConnection())
            {
                var cmd = new SqlCommand(@"
            SELECT COUNT(*) 
            FROM foProcessSteps 
            WHERE ProcessID = @ProcessID AND StepNo = @StepNo AND ID <> @ID", conn);

                cmd.Parameters.AddWithValue("@ProcessID", model.ProcessID);
                cmd.Parameters.AddWithValue("@StepNo", model.StepNo);
                cmd.Parameters.AddWithValue("@ID", model.ID);
                conn.Open();
                isDuplicate = (int)cmd.ExecuteScalar() > 0;
            }

            if (isDuplicate)
            {
                ModelState.AddModelError("StepNo", $"Step No {model.StepNo} already exists in this process.");
                ViewBag.HighlightStepNo = model.StepNo;
            }

            if (!ModelState.IsValid)
            {
                ViewBag.ProcessDropdown = GetProcessSelectList();
                ViewBag.GroupDropdown = GetGroupSelectList();
                ViewBag.UserDropdown = GetUserSelectList();
                ViewBag.ProcessStepList = GetStepsForProcess(model.ProcessID);
                return View(model);
            }

            using (var conn = GetConnection())
            {
                var cmd = new SqlCommand(@"
            UPDATE foProcessSteps SET 
                ProcessID = @ProcessID,
                StepNo = @StepNo,
                GroupID = @GroupID,
                UserID = @UserID,
                StepDescription = @StepDescription,
                Active = @Active
            WHERE ID = @ID", conn);

                cmd.Parameters.AddWithValue("@ID", model.ID);
                cmd.Parameters.AddWithValue("@ProcessID", model.ProcessID);
                cmd.Parameters.AddWithValue("@StepNo", model.StepNo);
                cmd.Parameters.AddWithValue("@GroupID", (object?)model.AssignGroupID ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@UserID", (object?)model.AssignUserID ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@StepDescription", (object?)model.StepDescription ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Active", model.Active);

                conn.Open();
                cmd.ExecuteNonQuery();
            }

            if (action == "addanother")
            {
                return RedirectToAction("Create", new { processid = model.ProcessID, userid });
            }

            return RedirectToAction("Index", new { processid = model.ProcessID, userid });
        }


        [HttpPost]
        public IActionResult Delete(long id, long processid, int userid)
        {
            SetUserAccess(userid);
            ViewBag.userid = userid;

            using (var conn = GetConnection())
            {
                var cmd = new SqlCommand("DELETE FROM foProcessSteps WHERE ID = @ID", conn);
                cmd.Parameters.AddWithValue("@ID", id);
                conn.Open();
                cmd.ExecuteNonQuery();
            }

            TempData["Message"] = "✅ Step deleted.";
            return RedirectToAction("Index", new { processid, userid });
        }

        private List<SelectListItem> GetProcessSelectList()
        {
            var list = new List<SelectListItem>();
            using (var conn = GetConnection())
            {
                var cmd = new SqlCommand("SELECT ID, ProcessName FROM foProcess ORDER BY ProcessName", conn);
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
            }
            return list;
        }

        private List<SelectListItem> GetGroupSelectList()
        {
            var list = new List<SelectListItem>();
            using (var conn = GetConnection())
            {
                var cmd = new SqlCommand("SELECT ID, Description FROM foGroups ORDER BY Description", conn);
                conn.Open();

                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    list.Add(new SelectListItem
                    {
                        Value = reader["ID"].ToString(),
                        Text = reader["Description"].ToString()
                    });
                }
            }
            return list;
        }

        private List<SelectListItem> GetUserSelectList()
        {
            var list = new List<SelectListItem>();
            using (var conn = GetConnection())
            {
                var cmd = new SqlCommand("SELECT ID, FirstName + ' ' + LastName as UserName FROM foUsers ORDER BY FirstName", conn);
                conn.Open();

                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    list.Add(new SelectListItem
                    {
                        Value = reader["ID"].ToString(),
                        Text = reader["UserName"].ToString()
                    });
                }
            }
            return list;
        }

        private Dictionary<long, List<foProcessDetail>> GetStepDetailsForProcess(long processid)
        {
            var map = new Dictionary<long, List<foProcessDetail>>();

            using var conn = GetConnection();
            var cmd = new SqlCommand(@"
                SELECT d.*
                FROM foProcessDetail d
                INNER JOIN foProcessSteps ps ON d.StepID = ps.ID
                WHERE ps.ProcessID = @ProcessID
                ORDER BY d.ID", conn);

            cmd.Parameters.AddWithValue("@ProcessID", processid);
            conn.Open();

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var detail = new foProcessDetail
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

                if (!map.ContainsKey(detail.StepID))
                    map[detail.StepID] = new List<foProcessDetail>();

                map[detail.StepID].Add(detail);
            }

            return map;
        }

        private List<foProcessSteps> GetStepsForProcess(long processid)
        {
            var list = new List<foProcessSteps>();

            using (var conn = GetConnection())
            {
                var cmd = new SqlCommand(@"
            SELECT ps.*, 
                   g.Description AS GroupName,
                   u.FirstName + ' ' + u.LastName AS UserName
            FROM foProcessSteps ps
            LEFT JOIN foGroups g ON ps.GroupID = g.ID
            LEFT JOIN foUsers u ON ps.UserID = u.ID
            WHERE ps.ProcessID = @ProcessID
            ORDER BY ps.StepNo", conn);

                cmd.Parameters.AddWithValue("@ProcessID", processid);
                conn.Open();

                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    list.Add(new foProcessSteps
                    {
                        ID = (long)reader["ID"],
                        ProcessID = (long)reader["ProcessID"],
                        StepNo = (decimal)reader["StepNo"],
                        AssignGroupID = reader["GroupID"] as long?,
                        AssignUserID = reader["UserID"] as long?,
                        StepDescription = reader["StepDescription"]?.ToString(),
                        GroupName = reader["GroupName"]?.ToString(),
                        UserName = reader["UserName"]?.ToString(),
                        Active = reader["Active"] != DBNull.Value && (bool)reader["Active"]
                    });
                }
            }

            return list;
        }

        public IActionResult Validate(long processid, int userid)
        {
            SetUserAccess(userid);
            var steps = GetStepsForProcess(processid);
            var detailMap = GetStepDetailsForProcess(processid);
            var errors = new List<string>();

            foreach (var step in steps)
            {
                var hasDetails = detailMap.ContainsKey(step.ID) && detailMap[step.ID].Any();
                if (!hasDetails)
                    errors.Add($"❌ Step {step.StepNo}: has no detail records.");

                if (hasDetails)
                {
                    var details = detailMap[step.ID];
                    if (!details.Any(d => d.Parent))
                        errors.Add($"❌ Step {step.StepNo}: missing a parent detail record.");

                    foreach (var d in details)
                    {
                        if (step.AssignGroupID == null && step.AssignUserID == null)
                            errors.Add($"❌ Step {step.StepNo}: requires GroupID or UserID to be assigned.");
                    }
                }
            }

            if (errors.Any())
            {
                TempData["ErrorList"] = errors;
            }
            else
            {
                TempData["Message"] = "✅ All steps and details validated successfully.";
            }

            return RedirectToAction("Index", new { processid, userid });
        }





    }
}

