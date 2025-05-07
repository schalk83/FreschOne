using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using FreschOne.Models;
using System.Data;
using FreschOne.Helpers;

namespace FreschOne.Controllers
{
    public class foEmailTemplateController : BaseController
    {
        public foEmailTemplateController(DatabaseHelper dbHelper, IConfiguration configuration)
            : base(dbHelper, configuration) { }

        private SqlConnection GetConnection() =>
            new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

        public IActionResult Index(int userid)
        {
            SetUserAccess(userid);
            ViewBag.userid = userid;

            var templates = new List<foEmailTemplate>();

            using (var conn = GetConnection())
            {
                var cmd = new SqlCommand("SELECT ID, TemplateType, EmailSubject, EmailBody, Active FROM foEmailTemplate " 
                    //+
                    //"WHERE Active = 1"
                    , conn);
                conn.Open();
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    templates.Add(new foEmailTemplate
                    {
                        ID = (long)reader["ID"],
                        TemplateType = reader["TemplateType"]?.ToString(),
                        EmailSubject = reader["EmailSubject"]?.ToString(),
                        EmailBody = reader["EmailBody"]?.ToString(),
                        Active = (bool)reader["Active"]
                    });
                }
            }

            return View(templates);
        }

        public IActionResult Create(int userid)
        {
            SetUserAccess(userid);
            ViewBag.userid = userid;
            ViewBag.TemplateTypes = TemplateTypes.GetAll();
            return View(new foEmailTemplate { Active = true });
        }

        [HttpPost]
        public IActionResult Create(foEmailTemplate model, int userid)
        {
            SetUserAccess(userid);
            ViewBag.userid = userid;
            ViewBag.TemplateTypes = TemplateTypes.GetAll();

            using (var conn = GetConnection())
            {
                var cmd = new SqlCommand(@"
                    INSERT INTO foEmailTemplate (TemplateType, EmailSubject, EmailBody, ActiveDate, Active)
                    VALUES (@Type, @Subject, @Body, GETDATE(), 1)", conn);

                cmd.Parameters.AddWithValue("@Type", model.TemplateType ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Subject", model.EmailSubject ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Body", model.EmailBody ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Active", model.Active);


                conn.Open();
                cmd.ExecuteNonQuery();
            }

            TempData["Message"] = "✅ Template created successfully.";
            return RedirectToAction("Index", new { userid });
        }

        public IActionResult Edit(long id, int userid)
        {
            SetUserAccess(userid);
            ViewBag.userid = userid;
            ViewBag.TemplateTypes = TemplateTypes.GetAll();
            foEmailTemplate model = null;

            using (var conn = GetConnection())
            {
                var cmd = new SqlCommand("SELECT * FROM foEmailTemplate WHERE ID = @ID", conn);
                cmd.Parameters.AddWithValue("@ID", id);
                conn.Open();

                using var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    model = new foEmailTemplate
                    {
                        ID = (long)reader["ID"],
                        TemplateType = reader["TemplateType"]?.ToString(),
                        EmailSubject = reader["EmailSubject"]?.ToString(),
                        EmailBody = reader["EmailBody"]?.ToString(),
                        Active = (bool)reader["Active"]
                    };
                }
            }

            return View(model);
        }

        [HttpPost]
        public IActionResult Edit(foEmailTemplate model, int userid)
        {
            SetUserAccess(userid);
            ViewBag.userid = userid;
            ViewBag.TemplateTypes = TemplateTypes.GetAll();
            using (var conn = GetConnection())
            {
                var cmd = new SqlCommand(@"
                    UPDATE foEmailTemplate 
                    SET TemplateType = @Type, EmailSubject = @Subject, EmailBody = @Body ,
                    Active = @Active 
                    WHERE ID = @ID", conn);

                cmd.Parameters.AddWithValue("@Type", model.TemplateType ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Subject", model.EmailSubject ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Body", model.EmailBody ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@ID", model.ID);
                cmd.Parameters.AddWithValue("@Active", model.Active);

                conn.Open();
                cmd.ExecuteNonQuery();
            }

            TempData["Message"] = "✅ Template updated successfully.";
            return RedirectToAction("Index", new { userid });
        }

        [HttpPost]
        public IActionResult Delete(long id, int userid)
        {
            SetUserAccess(userid);
            ViewBag.userid = userid;

            using (var conn = GetConnection())
            {
                var cmd = new SqlCommand("UPDATE foEmailTemplate SET Active = False WHERE ID = @ID", conn);
                cmd.Parameters.AddWithValue("@ID", id);
                conn.Open();
                cmd.ExecuteNonQuery();
            }

            TempData["Message"] = "✅ Template deleted successfully.";
            return RedirectToAction("Index", new { userid });
        }
    }
}
