using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Data.SqlClient;

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

            // Allow unauthenticated access ONLY to Account/Login and Account/ResetPassword (optional)
            bool isPublic = controller == "Account" &&
                           (action == "Login" || action == "ResetPassword" || action == "Logout");

            var isLoggedIn = context.HttpContext.Session.GetString("IsLoggedIn");
            var sessionUserId = context.HttpContext.Session.GetInt32("UserID");

            // 🔒 Redirect to login if not logged in and trying to access protected pages
            if (!isPublic && (isLoggedIn != "true" || sessionUserId == null))
            {
                context.Result = new RedirectToActionResult("Login", "Account", null);
                return;
            }

            // 🛡️ Prevent userId spoofing in query (only check if user is already logged in)
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

            // ✅ Set ViewBag.userid for consistent use
            ViewBag.userid = sessionUserId;

            base.OnActionExecuting(context);
        }
    }
}
