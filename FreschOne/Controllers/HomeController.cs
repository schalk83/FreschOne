using FreschOne.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace FreschOne.Controllers
{
    public class HomeController : BaseController
    {
        public HomeController(DatabaseHelper dbHelper, IConfiguration configuration) : base(dbHelper, configuration) { }

        public IActionResult Index(int userid)
        {
            SetUserAccess(userid);
            ViewBag.userid = userid; 
            return View();
        }
    }

}