using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using FreschOne.Models;
using System.Collections.Generic;

namespace FreschOne.Controllers
{
    public class foTablePrefixesController : BaseController
    {
        public foTablePrefixesController(DatabaseHelper dbHelper, IConfiguration configuration)
            : base(dbHelper, configuration) { }

        private SqlConnection GetConnection() => new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

        public IActionResult Index(int userid)
        {
            SetUserAccess(userid);
            ViewBag.userid = userid;

            var list = new List<foTablePrefixes>();

            using (var conn = GetConnection())
            {
                var cmd = new SqlCommand("SELECT ID, Prefix, Description, Active FROM foTablePrefixes", conn);
                conn.Open();

                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    list.Add(new foTablePrefixes
                    {
                        ID = (long)reader["ID"],
                        Prefix = reader["Prefix"]?.ToString(),
                        Description = reader["Description"]?.ToString(),
                        Active = reader["Active"] != DBNull.Value && (bool)reader["Active"]
                    });
                }
            }

            return View(list);
        }

        public IActionResult Create(int userid)
        {
            SetUserAccess(userid);
            ViewBag.userid = userid;
            return View();
        }

        [HttpPost]
        public IActionResult Create(foTablePrefixes model, int userid)
        {
            SetUserAccess(userid);
            ViewBag.userid = userid;

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            using (var conn = GetConnection())
            {
                var cmd = new SqlCommand("INSERT INTO foTablePrefixes (Prefix, Description, Active) VALUES (@Prefix, @Description, @Active)", conn);
                cmd.Parameters.AddWithValue("@Prefix", model.Prefix);
                cmd.Parameters.AddWithValue("@Description", model.Description);
                cmd.Parameters.AddWithValue("@Active", model.Active);
                conn.Open();
                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("Index", new { userid });
        }

        public IActionResult Edit(long id, int userid)
        {
            SetUserAccess(userid);
            ViewBag.userid = userid;

            foTablePrefixes model = null;

            using (var conn = GetConnection())
            {
                var cmd = new SqlCommand("SELECT * FROM foTablePrefixes WHERE ID = @ID", conn);
                cmd.Parameters.AddWithValue("@ID", id);
                conn.Open();

                using var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    model = new foTablePrefixes
                    {
                        ID = (long)reader["ID"],
                        Prefix = reader["Prefix"]?.ToString(),
                        Description = reader["Description"]?.ToString(),
                        Active = reader["Active"] != DBNull.Value && (bool)reader["Active"]
                    };
                }
            }

            return View(model);
        }

        [HttpPost]
        public IActionResult Edit(foTablePrefixes model, int userid)
        {
            SetUserAccess(userid);
            ViewBag.userid = userid;

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            using (var conn = GetConnection())
            {
                var cmd = new SqlCommand("UPDATE foTablePrefixes SET Prefix = @Prefix, Description = @Description, Active = @Active WHERE ID = @ID", conn);
                cmd.Parameters.AddWithValue("@ID", model.ID);
                cmd.Parameters.AddWithValue("@Prefix", model.Prefix);
                cmd.Parameters.AddWithValue("@Description", model.Description);
                cmd.Parameters.AddWithValue("@Active", model.Active);
                conn.Open();
                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("Index", new { userid });
        }

        [HttpPost]
        public IActionResult Delete(long id, int userid)
        {
            SetUserAccess(userid);
            ViewBag.userid = userid;

            using (var conn = GetConnection())
            {
                var cmd = new SqlCommand("DELETE FROM foTablePrefixes WHERE ID = @ID", conn);
                cmd.Parameters.AddWithValue("@ID", id);
                conn.Open();
                cmd.ExecuteNonQuery();
            }

            TempData["Message"] = "✅ Prefix deleted successfully.";
            return RedirectToAction("Index", new { userid });
        }
    }
}
