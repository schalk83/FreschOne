using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Data.SqlClient;
using System.Text;

namespace FreschOne.Controllers
{
    public class BaseController : Controller
    {
        protected readonly DatabaseHelper _dbHelper;
        protected readonly IConfiguration _configuration;

        public BaseController(DatabaseHelper dbHelper, IConfiguration configuration)
        {
            _dbHelper = dbHelper;
            _configuration = configuration;
        }

        protected void SetUserAccess(long userId)
        {
            bool isAdmin = _dbHelper.IsUserAdmin(userId);
            ViewBag.IsAdmin = isAdmin;
        }

        protected void GetUserReadWriteAccess(long userId, string tablename)
        {
            string readwriteaccess = _dbHelper.CheckUserTableAccessRights(userId, tablename);
            ViewBag.readwriteaccess = readwriteaccess;
        }

        protected void EnsureAuditFieldsExist(string tablename)
        {
            _dbHelper.EnsureAuditFieldsExist(tablename);
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var controller = context.ActionDescriptor.RouteValues["controller"];
            var action = context.ActionDescriptor.RouteValues["action"];

            bool isPublic = controller == "Account" &&
                           (action == "Login" || action == "ResetPassword" || action == "Logout");

            var isLoggedIn = context.HttpContext.Session.GetString("IsLoggedIn");
            var sessionUserId = context.HttpContext.Session.GetInt32("UserID");

            if (!isPublic && (isLoggedIn != "true" || sessionUserId == null))
            {
                context.Result = new RedirectToActionResult("Login", "Account", null);
                return;
            }

            if (isLoggedIn == "true" && sessionUserId != null &&
                context.HttpContext.Request.Query.ContainsKey("userId"))
            {
                if (int.TryParse(context.HttpContext.Request.Query["userId"], out int queryUserId))
                {
                    if (queryUserId != sessionUserId)
                    {
                        Console.WriteLine($"🚨 Spoofed userId: {queryUserId} ≠ session: {sessionUserId}");
                        context.Result = new RedirectToActionResult("Login", "Account", null);
                        return;
                    }
                }
            }

            // ✅ License Check: Decode and Validate
            string licenseEndDateString = GetDecodedLicenseKey();
            if (!string.IsNullOrEmpty(licenseEndDateString) && DateTime.TryParse(licenseEndDateString, out DateTime licenseEndDate))
            {
                if (licenseEndDate < DateTime.Now)
                {
                    context.Result = new ContentResult
                    {
                        Content = $"❌ License expired on {licenseEndDate:yyyy-MM-dd}. Please contact support.",
                        StatusCode = 403
                    };
                    return;
                }
            }
            else
            {
                context.Result = new ContentResult
                {
                    Content = "❌ Invalid or missing license. Please contact support.",
                    StatusCode = 403
                };
                return;
            }

            // ✅ Check critical tables (only if logged in)
            if (isLoggedIn == "true" && sessionUserId != null)
            {
                using (var conn = _dbHelper.GetConnection())
                {
                    conn.Open();
                    CheckCriticalTables(conn);
                }
            }

            ViewBag.userid = sessionUserId;
            base.OnActionExecuting(context);
        }

        private void CheckCriticalTables(SqlConnection conn)
        {
            var cmd = new SqlCommand(@"
        SELECT foSiteCheckDone, foSiteCheckTables 
        FROM foLicenseManagement ", conn);

            using var reader = cmd.ExecuteReader();
            bool checkDone = false;
            string tableList = null;

            if (reader.Read())
            {
                checkDone = reader["foSiteCheckDone"] != DBNull.Value && (bool)reader["foSiteCheckDone"];
                tableList = reader["foSiteCheckTables"]?.ToString();
            }

            reader.Close();

            if (checkDone || string.IsNullOrEmpty(tableList))
            {
                // ✅ No need to check or no tables listed
                return;
            }

            var tables = tableList.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            foreach (var table in tables)
            {
                var checkCmd = new SqlCommand(@"
            IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = @TableName)
                THROW 50001, '❌ Critical table not found: @TableName', 1;", conn);

                checkCmd.Parameters.AddWithValue("@TableName", table);
                checkCmd.ExecuteNonQuery();
            }

            // ✅ Mark site check as done
            var updateCmd = new SqlCommand("UPDATE foLicenseManagement SET foSiteCheckDone = 1 ", conn);
            updateCmd.ExecuteNonQuery();
        }

        public string DecodeLicenseKey(string licenseKey)
        {
            if (string.IsNullOrEmpty(licenseKey)) return null;

            var parts = licenseKey.Split('-');
            if (parts.Length != 2) return null;

            string encodedPart = parts[0];
            if (!int.TryParse(parts[1], out int seed)) return null;

            var numbers = new List<int>();
            var currentNumber = new StringBuilder();

            foreach (char c in encodedPart)
            {
                if (char.IsDigit(c))
                    currentNumber.Append(c);
                else if (currentNumber.Length > 0)
                {
                    numbers.Add(int.Parse(currentNumber.ToString()));
                    currentNumber.Clear();
                }
            }

            var decodedChars = new List<char>();
            for (int i = 0; i < numbers.Count; i++)
            {
                int multiplier = (2 * i) + seed;
                int ascii = numbers[i] / multiplier;
                decodedChars.Add((char)ascii);
            }

            return new string(decodedChars.ToArray());
        }

        public string GetDecodedLicenseKey()
        {
            string licenseKey = null;

            using (var conn = _dbHelper.GetConnection())  // ✅ The correct way!
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT LicenseKey FROM foLicenseManagement ", conn);
                licenseKey = cmd.ExecuteScalar()?.ToString();
            }

            return DecodeLicenseKey(licenseKey);
        }

    }
    public static class SqlDataReaderExtensions
    {
        public static bool HasColumn(this SqlDataReader reader, string columnName)
        {
            for (int i = 0; i < reader.FieldCount; i++)
            {
                if (reader.GetName(i).Equals(columnName, StringComparison.OrdinalIgnoreCase))
                    return true;
            }
            return false;
        }
    }

   


}
