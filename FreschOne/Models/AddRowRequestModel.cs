using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using FreschOne.Models;

namespace FreschOne.Models
{
    public class AddRowRequestModel
    {
        public string TableName { get; set; }
        public int StepId { get; set; }
        public int UserId { get; set; }
        public int? ProcessInstanceId { get; set; }
        public int CurrentRowCount { get; set; }


    }

}
