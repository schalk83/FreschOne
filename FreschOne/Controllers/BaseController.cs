using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Data.SqlClient;
using System.Net;
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
            //var controller = context.ActionDescriptor.RouteValues["controller"];
            //var action = context.ActionDescriptor.RouteValues["action"];

            //bool isPublic = controller == "Account" &&
            //               (action == "Login" || action == "ResetPassword" || action == "Logout");

            //var isLoggedIn = context.HttpContext.Session.GetString("IsLoggedIn");
            //var sessionUserId = context.HttpContext.Session.GetInt32("UserID");

            //if (!isPublic && (isLoggedIn != "true" || sessionUserId == null))
            //{
            //    context.Result = new RedirectToActionResult("Login", "Account", null);
            //    return;
            //}

            //if (isLoggedIn == "true" && sessionUserId != null &&
            //    context.HttpContext.Request.Query.ContainsKey("userId"))
            //{
            //    if (int.TryParse(context.HttpContext.Request.Query["userId"], out int queryUserId))
            //    {
            //        if (queryUserId != sessionUserId)
            //        {
            //            Console.WriteLine($"🚨 Spoofed userId: {queryUserId} ≠ session: {sessionUserId}");
            //            context.Result = new RedirectToActionResult("Login", "Account", null);
            //            return;
            //        }
            //    }
            //}

            //// ✅ License Check: Decode and Validate
            //string licenseEndDateString = GetDecodedLicenseKey();
            //if (!string.IsNullOrEmpty(licenseEndDateString) && DateTime.TryParse(licenseEndDateString, out DateTime licenseEndDate))
            //{
            //    if (licenseEndDate < DateTime.Now)
            //    {
            //        context.Result = new ContentResult
            //        {
            //            Content = $"❌ License expired on {licenseEndDate:yyyy-MM-dd}. Please contact support.",
            //            StatusCode = 403
            //        };
            //        return;
            //    }
            //}
            //else
            //{
            //    context.Result = new ContentResult
            //    {
            //        Content = "❌ Invalid or missing license. Please contact support.",
            //        StatusCode = 403
            //    };
            //    return;
            //}

            //// ✅ Check critical tables (only if logged in)
            //if (isLoggedIn == "true" && sessionUserId != null)
            //{
            //    using (var conn = _dbHelper.GetConnection())
            //    {
            //        conn.Open();
            //        CheckCriticalTables(conn);
            //    }
            //}

            //ViewBag.userid = sessionUserId;
            //base.OnActionExecuting(context);
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

            // Replace all letters with -
            var cleanEncoded = new string(encodedPart.Select(c => char.IsLetter(c) ? '-' : c).ToArray());

            // Split on -
            var numberStrings = cleanEncoded.Split('-', StringSplitOptions.RemoveEmptyEntries);
            var numbers = numberStrings.Select(int.Parse).ToList();

            var decodedChars = new List<char>();
            for (int i = 0; i < numbers.Count; i++)
            {
                int divisor = (i + (i + seed));
                int ascii = (numbers[i] / divisor);
                decodedChars.Add((char)ascii);
            }

            return new string(decodedChars.ToArray());
        }


        /*     static string DecodeEndDate(string l_EndDate)
             {

                 int li_amount = 0;

                 try
                 {
                     li_amount = int.Parse(DBCredentials.Amount);
                 }
                 catch
                 {
                     MessageBox.Show("Invalid iteration. License key invalid");
                     Application.Exit();
                 }

                 string[] lsa_EndDate = new string[l_EndDate.Length];
                 for (int i = 0; i < l_EndDate.Length; i++)
                 {
                     lsa_EndDate[i] = l_EndDate[i].ToString();

                 }

                 for (int i = 0; i < lsa_EndDate.Length; i++)
                 {
                     try
                     {
                         Convert.ToInt32(lsa_EndDate[i].ToString());
                     }
                     catch
                     {
                         l_EndDate = l_EndDate.Replace(lsa_EndDate[i].ToString(), "-");
                     }
                 }

                 string l_inistring = "";
                 l_inistring = l_EndDate;

                 //Input string
                 string l_letters = "";
                 string l_password_arraycounter = l_inistring;
                 int li_x = 0;

                 while (l_password_arraycounter.Length > 0)
                 {
                     if (l_password_arraycounter.IndexOf("-") < 0)
                     {
                         l_password_arraycounter = "";
                     }
                     else
                     {
                         l_password_arraycounter = l_password_arraycounter.Remove(0, l_password_arraycounter.IndexOf("-") + 1);
                     }
                     li_x = li_x + 1;
                 }

                 string[] l_array = new string[li_x];

                 li_x = 0;
                 l_password_arraycounter = l_inistring;

                 while (l_password_arraycounter.Length > 0)
                 {
                     if (l_password_arraycounter.IndexOf("-") < 0)
                     {
                         l_array[li_x] = l_password_arraycounter;
                         l_password_arraycounter = "";
                     }
                     else
                     {
                         l_array[li_x] = l_password_arraycounter.Substring(0, l_password_arraycounter.IndexOf("-"));
                         l_password_arraycounter = l_password_arraycounter.Remove(0, l_password_arraycounter.IndexOf("-") + 1);
                     }
                     li_x = li_x + 1;
                 }


                 byte[] array = new byte[li_x];

                 for (int a = 0; a < l_array.Length; a++)
                 {
                     //USE THE A+5 dynamically out of the text
                     l_array[a] = ((int.Parse(l_array[a]) / (a + (a + li_amount))) + 7)).ToString();
                     array[a] = byte.Parse(l_array[a].ToString());
                 }

                 l_letters = ASCIIEncoding.ASCII.GetString(array);

                 return l_letters;
             }*/

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
