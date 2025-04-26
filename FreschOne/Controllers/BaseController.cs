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
             
    }
}
