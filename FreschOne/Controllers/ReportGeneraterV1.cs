//using Microsoft.AspNetCore.Mvc;
//using System.Data;

//namespace FreschOne.Controllers
//{
//    public class ReportGeneraterV1 : Controller
//    {
//        public IActionResult GenerateReport(DataTable dataset, string type)
//        {
//            if (type == "Table")
//            {
//                return View("TableReport", dataset);
//            }
//            else if (type == "Freeform")
//            {
//                return View("FreeformReport", dataset);
//            }
//            else
//            {
//                return BadRequest("Invalid report type.");
//            }
//        }

//    }
//}
