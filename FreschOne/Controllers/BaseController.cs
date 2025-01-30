using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace FreschOne.Controllers
{
    public class BaseController : Controller
    {
        protected readonly DatabaseHelper _dbHelper;
        protected readonly IConfiguration _configuration;

        public BaseController(DatabaseHelper dbHelper, IConfiguration configuration)
        {
            _dbHelper = dbHelper;
            _configuration = configuration;  // Injected IConfiguration
        }

        protected void SetUserAccess(long userId)
        {
            bool isAdmin = _dbHelper.IsUserAdmin(userId);
            ViewBag.IsAdmin = isAdmin;
        }
    }
}
