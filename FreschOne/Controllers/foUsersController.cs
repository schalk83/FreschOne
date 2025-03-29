using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using FreschOne.Models;
using System.Collections.Generic;
using System.Data;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;
using DocumentFormat.OpenXml.Spreadsheet;


namespace FreschOne.Controllers
{
    public class foUsersController : BaseController
    {
        public foUsersController(DatabaseHelper dbHelper, IConfiguration configuration) : base(dbHelper, configuration) { }

        private SqlConnection GetConnection() => new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

        public IActionResult Index(int userid)
        {
            SetUserAccess(userid);
            ViewBag.userid = userid;


            var users = new List<foUser>();

            using (var conn = GetConnection())
            {
                var cmd = new SqlCommand("SELECT * FROM foUsers WHERE Active = 1", conn);
                conn.Open();
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    users.Add(new foUser
                    {
                        ID = (long)reader["ID"],
                        UserName = reader["UserName"].ToString(),
                        Password = reader["Password"].ToString(),
                        FirstName = reader["FirstName"].ToString(),
                        LastName = reader["LastName"].ToString(),
                        Email = reader["Email"].ToString(),
                        Admin = Convert.ToBoolean(reader["Admin"]),
                        Active = Convert.ToBoolean(reader["Active"])
                    });
                }
            }

            return View(users);
        }

        public IActionResult Create(int userid)
        {
            SetUserAccess(userid);
            ViewBag.userid = userid;

            return View(new foUser());
        }

        [HttpPost]
        public IActionResult Create(foUser user, int userid)
        {

            if (!ModelState.IsValid)
            {
                return View(user);
            }

            using (var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                var cmd = new SqlCommand(@"
            INSERT INTO foUsers (UserName, Password, FirstName, LastName, Email, Admin, Active)
            VALUES (@UserName, @Password, @FirstName, @LastName, @Email, @Admin, 1)", conn);

                cmd.Parameters.AddWithValue("@UserName", user.UserName);
                cmd.Parameters.AddWithValue("@Password", user.Password);
                cmd.Parameters.AddWithValue("@FirstName", (object?)user.FirstName ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@LastName", (object?)user.LastName ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Email", (object?)user.Email ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Admin", user.Admin);

                conn.Open();
                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("Index",new { userid });
        }


        public IActionResult Edit(long id, int userid)
        {
            SetUserAccess(userid);
            ViewBag.userid = userid;

            foUser user = null;

            using (var conn = GetConnection())
            {
                var cmd = new SqlCommand("SELECT * FROM foUsers WHERE ID = @ID", conn);
                cmd.Parameters.AddWithValue("@ID", id);
                conn.Open();

                using var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    user = new foUser
                    {
                        ID = (long)reader["ID"],
                        UserName = reader["UserName"].ToString(),
                        Password = reader["Password"].ToString(),
                        FirstName = reader["FirstName"].ToString(),
                        LastName = reader["LastName"].ToString(),
                        Email = reader["Email"].ToString(),
                        Admin = Convert.ToBoolean(reader["Admin"]),
                        Active = Convert.ToBoolean(reader["Active"])
                    };
                }
            }

            return View(user);
        }

        [HttpPost]
        public IActionResult Edit(foUser user, int userid)
        {
            using (var conn = GetConnection())
            {
                var cmd = new SqlCommand(@"
                    UPDATE foUsers SET 
                        UserName = @UserName,
                        Password = @Password,
                        FirstName = @FirstName,
                        LastName = @LastName,
                        Email = @Email,
                        Admin = @Admin
                    WHERE ID = @ID", conn);

                cmd.Parameters.AddWithValue("@UserName", user.UserName);
                cmd.Parameters.AddWithValue("@Password", user.Password);
                cmd.Parameters.AddWithValue("@FirstName", (object?)user.FirstName ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@LastName", (object?)user.LastName ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Email", (object?)user.Email ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Admin", user.Admin);
                cmd.Parameters.AddWithValue("@ID", user.ID);

                conn.Open();
                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("Index", new {userid});
        }
        [HttpPost]
        public IActionResult Delete(long id, int userid)
        {
            using (var conn = GetConnection())
            {
                conn.Open();

                // Check if user ID exists in any related table
                var tablesToCheck = new[]
                {
            "foUserLogin",
            "foUserPasswordReset",
            "foUserReports",
            "foUserTable"
        };

                foreach (var table in tablesToCheck)
                {
                    var cmd = new SqlCommand($"SELECT COUNT(*) FROM {table} WHERE UserID = @UserID", conn);
                    cmd.Parameters.AddWithValue("@UserID", id);
                    int count = (int)cmd.ExecuteScalar();

                    if (count > 0)
                    {
                        TempData["ErrorMessage"] = $"❌ This user is linked to {table} and cannot be deleted.";
                        return RedirectToAction("Index");
                    }
                }

                // Safe to soft delete
                var deleteCmd = new SqlCommand("UPDATE foUsers SET Active = 0 WHERE ID = @ID", conn);
                deleteCmd.Parameters.AddWithValue("@ID", id);
                deleteCmd.ExecuteNonQuery();

                TempData["Message"] = "✅ User deleted successfully.";
            }

            return RedirectToAction("Index", new { userid });
        }

    }
}
