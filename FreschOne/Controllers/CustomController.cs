using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace FreschOne.Controllers
{
    public class CustomEventsController : BaseController
    {
        public CustomEventsController(DatabaseHelper dbHelper, IConfiguration configuration) : base(dbHelper, configuration) { }
        private SqlConnection GetConnection() => new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

        public IActionResult Index(int stepid, string tablename)
        {
            if ( stepid == 1 && tablename == "tbl_tran_Student" )
            {


            }

            return View();
        }
    }
}
