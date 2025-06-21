using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using FreschOne.Models;


namespace FreschOne.Controllers
{
    public class foEventLogicController : BaseController
    {
        public foEventLogicController(DatabaseHelper dbHelper, IConfiguration configuration) : base(dbHelper, configuration) { }

        private SqlConnection GetConnection() => new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

        [HttpPost]
        [Route("foEventLogic/RefreshAsync")]
        public IActionResult RefreshAsync([FromQuery] string changedField, [FromBody] RefreshRequest payload)
        {
            if (payload?.TableName == null || payload.Row == null)
                return BadRequest("Missing data.");

            var tableName = payload.TableName;
            tableName = tableName + "_";
            var row = payload.Row;

            var result = new Dictionary<string, object>();
            var fieldname = "";

            if (tableName == "tbl_tran_Employee")
            {
                fieldname = "FirstName";

                if (row.TryGetValue(tableName + fieldname, out var firstNameToken) && firstNameToken?.ToString() == "Schalk")
                {
                    result[tableName + "LastName"] = new
                    {
                        visible = true,
                        value = "Van der Merwe",
                        readOnly = true
                    };
                }
                

                fieldname = "IDNumber";
                if (row.TryGetValue(tableName + fieldname, out var IDNumberToken) && IDNumberToken?.ToString() == "123")
                {
                    result[tableName + "Email"] = new
                    {
                        visible = false,
                        value = "schalk@123.com",
                        readOnly = false,
                        
                    };
                }
                else
                {
                    result[tableName + "Email"] = new
                    {
                        visible = true,
                        value = "Western Cape",
                        readOnly = true
                    };
                }
            }


            return Json(result);
        }



    }
}