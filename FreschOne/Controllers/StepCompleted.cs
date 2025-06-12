using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace FreschOne.Controllers
{
    public class StepCompletedController : BaseController
    {
        public StepCompletedController(DatabaseHelper dbHelper, IConfiguration configuration) : base(dbHelper, configuration) { }
        
        public IActionResult StepCompleted(string message, int userId, string actionheader, string processInstanceID)
        {

            ViewBag.Message = message;
            ViewBag.UserId = userId;
            ViewBag.action = actionheader;
            ViewBag.processInstanceID = processInstanceID;
            return View();
        }

    }
}
